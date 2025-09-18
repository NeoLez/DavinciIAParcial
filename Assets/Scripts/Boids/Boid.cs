using System;
using Boids.SO;
using DecisionTree;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Boids
{
    public class Boid : MovementAgent<BoidSO> {
        public BoidNode uglyDecisionTree;
        private void Update() {
            if (Settings.highPerformanceMode) {
                DecisionTreeHighPerformance();
            }
            else {
                uglyDecisionTree.Execute(this);
            }
            
            AddVelocity(AvoidEscapingBounds());

            ProcessMovement();
        }

        private void DecisionTreeHighPerformance() {
            float foodDistance = Settings.foodDetectionRange;
            if (IsFoodNearby(out Target food, ref foodDistance)) {
                if (foodDistance <= food.radius) {
                    ConsumeFood(food);
                }
                else {
                    AddVelocity(Arrive(food.transform.position, food.radius));
                }
            } else if (IsHunterNearby(out Hunter hunter)) {
                AddVelocity(Flee(hunter.transform.position));
            } else if (AreBoidsNearby()) {
                AddVelocity(FlockingHighPerformance());
            }
            else {
                AddVelocity(RandomMovement());
            }
        }

        public void ConsumeFood(Target food) {
            Manager.instance.FoodItems.Remove(food);
            Destroy(food.gameObject);
            Reproduce();
            Manager.instance.amountBorn++;
        }
        
        /// <summary>
        /// Creates a clone of the <c>Boid</c>
        /// </summary>
        private void Reproduce() {
            GameObject newBoid = Instantiate(gameObject);
            newBoid.transform.position += (Vector3)Random.insideUnitCircle * 0.01f;
            Boid boidComponent = newBoid.GetComponent<Boid>();
            boidComponent.Initialize(Settings);
            boidComponent.SetInitialVelocity(velocity);
            Manager.instance.Boids.Add(boidComponent);
        }
        
        /// <param name="food">Returns the closest food item to the <c>Boid</c> which is also closer than minimumDistance. If not found, returns null</param>
        /// <param name="minimumDistance">Food items that are further from the <c>Boid</c> than this are ignored. The value is altered to match the distance from the <c>Boid</c> to the closest food item if found.</param>
        /// <returns>True if food was found. Otherwise, false.</returns>
        public bool IsFoodNearby(out Target food, ref float minimumDistance) {
            food = null;
            foreach (var foodItem in Manager.instance.FoodItems) {
                float dist = Vector2.Distance(transform.position, foodItem.transform.position);
                if (dist <= minimumDistance) {
                    food = foodItem;
                    minimumDistance = dist;
                }
            }

            if (food == null)
                return false;

            return true;
        }
        
        /// <param name="hunter">Returns the hunter's <c>Target</c> or null if none is found.</param>
        /// <returns>True if there is a <c>Hunter</c> within view range.</returns>
        public bool IsHunterNearby(out Hunter hunter) {
            float closestDistance = Single.MaxValue;
            Hunter newHunter = null;
            
            foreach (var h in Manager.instance.Hunters) {
                float distance = Vector2.Distance(transform.position, h.transform.position);
                if (distance < closestDistance) {
                    newHunter = h;
                    closestDistance = distance;
                }
            }

            if (closestDistance < Single.MaxValue) {
                if (Vector2.Distance(newHunter!.transform.position, transform.position) <= Settings.hunterDetectionRange) {
                    hunter = newHunter;
                    return true;
                }
            }

            hunter = null;
            return false;
        }
        
        /// <returns>True if <c>Boid</c>s are found within the alignment radius or cohesion radius, whichever is biggest.</returns>
        public bool AreBoidsNearby() {
            foreach (var boid in Manager.instance.Boids) {
                if (boid == this) continue;
                if (Vector2.Distance(boid.transform.position, transform.position) < Math.Max(Settings.alignmentRadius, Settings.cohesionRadius)) {
                    return true;
                }
            }

            return false;
        }

        /// <returns>A Seek vector toward the center of the map.</returns>
        private Vector2 AvoidEscapingBounds() {
            if (transform.position.magnitude > Settings.circleBoundRadius) {
                return Seek(Vector2.zero) * Settings.circleBoundStrength;
            }
            return Vector2.zero;
        }

        private Vector2 Alignment() {
            int alignmentCount = 0;
            Vector2 alignmentSum = Vector2.zero;
            foreach (var boid in Manager.instance.Boids) {
                if (boid == this) continue;
                Vector2 toOther = (Vector2)boid.transform.position - (Vector2)transform.position;
                float distance = toOther.magnitude;
                if (Vector2.Angle(velocity, boid.transform.position - transform.position) <= Settings.fov) {
                    if (distance <= Settings.alignmentRadius) {
                        alignmentCount++;
                        alignmentSum += boid.velocity;
                    }  
                }
            }
            Vector2 alignmentSteering = Vector2.zero;
            if (alignmentCount == 0) return alignmentSteering;
            Vector2 desired = alignmentSum.normalized * Settings.maxSpeed;
            alignmentSteering = Seek(desired + (Vector2)transform.position) * Settings.alignmentStrength;
            return alignmentSteering;
        }

        private Vector2 Cohesion() {
            int cohesionCount = 0;
            Vector2 cohesionSum = Vector2.zero;
            foreach (var boid in Manager.instance.Boids) {
                if (boid == this) continue;
                Vector2 toOther = (Vector2)boid.transform.position - (Vector2)transform.position;
                float distance = toOther.magnitude;
                if (distance <= Settings.cohesionRadius && Vector2.Angle(velocity, boid.transform.position - transform.position) <= Settings.fov) {
                    cohesionCount++;
                    cohesionSum += (Vector2)boid.transform.position;
                }
            }
            
            Vector2 cohesionSteering = Vector2.zero;
            if (cohesionCount == 0) return cohesionSteering;
            Vector2 center = cohesionSum / cohesionCount;
            cohesionSteering = Seek(center) * Settings.cohesionStrength;
            return cohesionSteering;
        }

        private Vector2 Separation() {
            int separationCount = 0;
            Vector2 separationSum = Vector2.zero;

            foreach (var boid in Manager.instance.Boids) {
                if (boid == this) continue;
                
                Vector2 toOther = (Vector2)boid.transform.position - (Vector2)transform.position;
                float distance = toOther.magnitude;
                
                if (distance <= Settings.separationRadius) {
                    separationCount++;
                    Vector2 away = -toOther.normalized;
                    float weight = Settings.separationRadius / distance;
                    separationSum += away * weight;
                }
            }

            Vector2 separationSteering = Vector2.zero;
            if (separationCount == 0) return separationSteering;
            separationSteering = Seek(separationSum + (Vector2)transform.position) * Settings.separationStrength;
            return separationSteering;
        }
        
        /// <summary>
        /// Calculates the flocking movement vector as the sum of the Separation, Alignment and Cohesion rules.
        /// </summary>
        /// <returns>Flocking movement vector.</returns>
        private Vector2 Flocking() {
            Vector2 randomSteer = Settings.randomStrength * Random.insideUnitCircle;
            return Separation() + Alignment() + Cohesion() + randomSteer;
        }

        private Vector2 _randomVector;
        private void Awake() {
            _randomVector = Random.insideUnitCircle;
        }
        
        /// <returns>A pseudorandom movement vector.</returns>
        public Vector2 RandomMovement() {
            if (Vector2.Angle(velocity, _randomVector) < 10 && velocity.magnitude > Settings.maxSpeed*0.95) {
                _randomVector = Random.insideUnitCircle;
            }

            return Seek((Vector2)transform.position + _randomVector) * 0.15f;
        }

        /// <summary>
        /// Set's the velocity of the entity. Meant to be used only when first creating it.
        /// </summary>
        public void SetInitialVelocity(Vector2 initialVelocity) {
            velocity = initialVelocity;
        }

        /// <summary>
        /// Calculates the flocking movement vector as the sum of the Separation, Alignment and Cohesion rules in a more computationally efficient way.
        /// </summary>
        /// <returns>Flocking movement vector.</returns>
        public Vector2 FlockingHighPerformance() {
            int alignmentCount = 0, separationCount = 0, cohesionCount = 0;
            Vector2 alignmentSum = Vector2.zero, separationSum = Vector2.zero, cohesionSum = Vector2.zero;

            foreach (var boid in Manager.instance.Boids) {
                if (boid == this) continue;
                
                Vector2 toOther = (Vector2)boid.transform.position - (Vector2)transform.position;
                float distance = toOther.magnitude;
                
                if (distance <= Settings.separationRadius) {
                    separationCount++;
                    Vector2 away = -toOther.normalized;
                    float weight = Settings.separationRadius / distance;
                    separationSum += away * weight;
                }
                else {
                    if (Vector2.Angle(velocity, boid.transform.position - transform.position) <= Settings.fov) {
                        if (distance <= Settings.cohesionRadius) {
                            cohesionCount++;
                            cohesionSum += (Vector2)boid.transform.position;
                        } 
                    }
                    
                }
                if (Vector2.Angle(velocity, boid.transform.position - transform.position) <= Settings.fov) {
                    if (distance <= Settings.alignmentRadius) {
                        alignmentCount++;
                        alignmentSum += boid.velocity;
                    }  
                }
                
            }

            Vector2 separationSteering = Vector2.zero;
            if (separationCount > 0) {
                separationSteering = Seek(separationSum + (Vector2)transform.position) * Settings.separationStrength;
            }

            Vector2 alignmentSteering = Vector2.zero;
            if (alignmentCount > 0) {
                Vector2 desired = alignmentSum.normalized * Settings.maxSpeed;
                alignmentSteering = Seek(desired + (Vector2)transform.position) * Settings.alignmentStrength;
            }

            Vector2 cohesionSteering = Vector2.zero;
            if (cohesionCount > 0) {
                Vector2 center = cohesionSum / cohesionCount;
                cohesionSteering = Seek(center) * Settings.cohesionStrength;
            }
            
            Vector2 randomSteer = Settings.randomStrength * Random.insideUnitCircle;
            return separationSteering + alignmentSteering + cohesionSteering + randomSteer;
        }
    }
}
