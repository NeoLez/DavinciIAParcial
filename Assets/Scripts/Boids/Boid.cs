using System;
using Boids.SO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boids
{
    public class Boid : MovementAgent {
        [NonSerialized] private BoidSO _settings;

        public void Initialize(BoidSO settings) {
            base.Initialize(settings);
            _settings = settings;
        }

        private void Update() {
            if (IsFoodNearby(out Target food)) {
                if (Vector2.Distance(food.position, transform.position) <= food.radius) {
                    ConsumeFood(food);
                }
                else {
                    AddVelocity(Arrive(food.position, food.radius));
                }
            } else if (IsHunterNearby(out Hunter hunter)) {
                AddVelocity(Flee(hunter.transform.position));
            } else if (AreBoidsNearby()) {
                if(_settings.highPerformanceMode)
                    AddVelocity(FlockingHighPerformance());
                else
                    AddVelocity(Flocking());
            }
            else {
                AddVelocity(RandomMovement());
            }
            
            AddVelocity(AvoidEscapingBounds());

            ProcessMovement();
        }

        private void ConsumeFood(Target food) {
            Manager.instance.FoodItems.Remove(food);
            Destroy(food.gameObject);
            Reproduce();
            Manager.instance.amountBorn++;
        }
        
        private void Reproduce() {
            GameObject newBoid = Instantiate(gameObject);
            newBoid.transform.position += (Vector3)Random.insideUnitCircle * 0.01f;
            Boid boidComponent = newBoid.GetComponent<Boid>();
            boidComponent.Initialize(_settings);
            Manager.instance.Boids.Add(boidComponent);
        }
        
        private bool IsFoodNearby(out Target food) {
            Target closestFood = null;
            float closestDistance = Single.MaxValue;
            foreach (var foodItem in Manager.instance.FoodItems) {
                float distance = Vector2.Distance(transform.position, foodItem.position);
                if (distance <= closestDistance) {
                    closestFood = foodItem;
                    closestDistance = distance;
                }
            }

            if (closestFood != null && closestDistance <= _settings.foodDetectionRange) {
                food = closestFood;
                return true;
            }

            food = null;
            return false;
        }

        private bool IsHunterNearby(out Hunter hunter) {
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
                if (Vector2.Distance(newHunter!.transform.position, transform.position) <= _settings.hunterDetectionRange) {
                    hunter = newHunter;
                    return true;
                }
            }

            hunter = null;
            return false;
        }

        private bool AreBoidsNearby() {
            foreach (var boid in Manager.instance.Boids) {
                if (boid == this) continue;
                if (Vector2.Distance(boid.transform.position, transform.position) < Math.Max(_settings.alignmentRadius, _settings.cohesionRadius)) {
                    return true;
                }
            }

            return false;
        }

        public Vector2 AvoidEscapingBounds() {
            if (transform.position.magnitude > _settings.circleBoundRadius) {
                return Seek(Vector2.zero) * _settings.circleBoundStrength;
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
                if (Vector2.Angle(velocity, boid.transform.position - transform.position) <= _settings.fov) {
                    if (distance <= _settings.alignmentRadius) {
                        alignmentCount++;
                        alignmentSum += boid.velocity;
                    }  
                }
            }
            Vector2 alignmentSteering = Vector2.zero;
            if (alignmentCount == 0) return alignmentSteering;
            Vector2 desired = alignmentSum.normalized * _settings.maxSpeed;
            alignmentSteering = Seek(desired + (Vector2)transform.position) * _settings.alignmentStrength;
            return alignmentSteering;
        }

        private Vector2 Cohesion() {
            int cohesionCount = 0;
            Vector2 cohesionSum = Vector2.zero;
            foreach (var boid in Manager.instance.Boids) {
                if (boid == this) continue;
                Vector2 toOther = (Vector2)boid.transform.position - (Vector2)transform.position;
                float distance = toOther.magnitude;
                if (distance <= _settings.cohesionRadius && Vector2.Angle(velocity, boid.transform.position - transform.position) <= _settings.fov) {
                    cohesionCount++;
                    cohesionSum += (Vector2)boid.transform.position;
                }
            }
            
            Vector2 cohesionSteering = Vector2.zero;
            if (cohesionCount == 0) return cohesionSteering;
            Vector2 center = cohesionSum / cohesionCount;
            cohesionSteering = Seek(center) * _settings.cohesionStrength;
            return cohesionSteering;
        }

        private Vector2 Separation() {
            int separationCount = 0;
            Vector2 separationSum = Vector2.zero;

            foreach (var boid in Manager.instance.Boids) {
                if (boid == this) continue;
                
                Vector2 toOther = (Vector2)boid.transform.position - (Vector2)transform.position;
                float distance = toOther.magnitude;
                
                if (distance <= _settings.separationRadius) {
                    separationCount++;
                    Vector2 away = -toOther.normalized;
                    float weight = _settings.separationRadius / distance;
                    separationSum += away * weight;
                }
            }

            Vector2 separationSteering = Vector2.zero;
            if (separationCount == 0) return separationSteering;
            separationSteering = Seek(separationSum + (Vector2)transform.position) * _settings.separationStrength;
            return separationSteering;
        }
        
        private Vector2 Flocking() {
            Vector2 randomSteer = _settings.randomStrength * Random.insideUnitCircle;
            return Separation() + Alignment() + Cohesion() + randomSteer;
        }

        private Vector2 _randomVector;
        private void Awake() {
            _randomVector = Random.insideUnitCircle;
        }
        
        private Vector2 RandomMovement() {
            if (Vector2.Angle(velocity, _randomVector) < 10 && velocity.magnitude > _settings.maxSpeed*0.95) {
                _randomVector = Random.insideUnitCircle;
            }

            return Seek((Vector2)transform.position + _randomVector) * 0.15f;
        }

        public void SetInitialVelocity(Vector2 initialVelocity) {
            velocity = initialVelocity;
        }

        private Vector2 FlockingHighPerformance() {
            int alignmentCount = 0, separationCount = 0, cohesionCount = 0;
            Vector2 alignmentSum = Vector2.zero, separationSum = Vector2.zero, cohesionSum = Vector2.zero;

            foreach (var boid in Manager.instance.Boids) {
                if (boid == this) continue;
                
                Vector2 toOther = (Vector2)boid.transform.position - (Vector2)transform.position;
                float distance = toOther.magnitude;
                
                if (distance <= _settings.separationRadius) {
                    separationCount++;
                    Vector2 away = -toOther.normalized;
                    float weight = _settings.separationRadius / distance;
                    separationSum += away * weight;
                }
                else {
                    if (Vector2.Angle(velocity, boid.transform.position - transform.position) <= _settings.fov) {
                        if (distance <= _settings.cohesionRadius) {
                            cohesionCount++;
                            cohesionSum += (Vector2)boid.transform.position;
                        } 
                    }
                    
                }
                if (Vector2.Angle(velocity, boid.transform.position - transform.position) <= _settings.fov) {
                    if (distance <= _settings.alignmentRadius) {
                        alignmentCount++;
                        alignmentSum += boid.velocity;
                    }  
                }
                
            }

            Vector2 separationSteering = Vector2.zero;
            if (separationCount > 0) {
                separationSteering = Seek(separationSum + (Vector2)transform.position) * _settings.separationStrength;
            }

            Vector2 alignmentSteering = Vector2.zero;
            if (alignmentCount > 0) {
                Vector2 desired = alignmentSum.normalized * _settings.maxSpeed;
                alignmentSteering = Seek(desired + (Vector2)transform.position) * _settings.alignmentStrength;
            }

            Vector2 cohesionSteering = Vector2.zero;
            if (cohesionCount > 0) {
                Vector2 center = cohesionSum / cohesionCount;
                cohesionSteering = Seek(center) * _settings.cohesionStrength;
            }
            
            Vector2 randomSteer = _settings.randomStrength * Random.insideUnitCircle;
            return separationSteering + alignmentSteering + cohesionSteering + randomSteer;
        }
    }
}
