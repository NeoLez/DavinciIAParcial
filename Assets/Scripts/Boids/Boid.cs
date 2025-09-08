using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    public class Boid : MovementAgent {
        [SerializeField] private Transform seekPos;
        [SubclassSelector] [SerializeReference] private List<Behaviour> behaviours;
        [SerializeField] private float alignmentRadius;
        [SerializeField] private float separationRadius;
        [SerializeField] private float cohesionRadius;
        [Range(0,1)] [SerializeField] private float alignmentStrength;
        [Range(0,1)] [SerializeField] private float separationStrength;
        [Range(0,1)] [SerializeField] private float cohesionStrength;
        [Range(0,1)] [SerializeField] private float randomStrength;
        private void Update() {
            int alignmentCount = 0, separationCount = 0, cohesionCount = 0;
            Vector2 alignmentSum = Vector2.zero, separationSum = Vector2.zero, cohesionSum = Vector2.zero;

            foreach (var boid in Manager.Instance.boids) {
                if (boid == this) continue;
                Vector2 toOther = (Vector2)boid.transform.position - (Vector2)transform.position;
                float distance = toOther.magnitude;
                
                if (distance <= separationRadius) {
                    separationCount++;
                    Vector2 away = -toOther.normalized;
                    float weight = separationRadius / distance;
                    separationSum += away * weight;
                }
                else {
                    if (distance <= cohesionRadius) {
                        cohesionCount++;
                        cohesionSum += (Vector2)boid.transform.position;
                    }
                }
                
                if (distance <= alignmentRadius) {
                    alignmentCount++;
                    alignmentSum += boid.velocity;
                }
            }

            Vector2 separationSteering = Vector2.zero;
            if (separationCount > 0) {
                separationSteering = Seek(separationSum) * separationStrength;
            }

            Vector2 alignmentSteering = Vector2.zero;
            if (alignmentCount > 0) {
                Vector2 desired = alignmentSum.normalized * maxSpeed;
                alignmentSteering = Seek(desired) * alignmentStrength;
            }

            Vector2 cohesionSteering = Vector2.zero;
            if (cohesionCount > 0) {
                Vector2 center = cohesionSum / cohesionCount;
                cohesionSteering = Seek(center) * cohesionStrength;
            }
            
            Vector2 randomSteer = randomStrength * Random.insideUnitCircle;
            velocity += separationSteering + alignmentSteering + cohesionSteering + randomSteer;

            ProcessMovement();
        }
    }
}
