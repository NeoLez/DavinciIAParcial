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
        private void Update() {
            int alignmentCount = 0, separationCount = 0, cohesionCount =0;
            Vector2 alignmentSteering = Vector2.zero, separationSteering = Vector2.zero, cohesionSteering = Vector2.zero;
            foreach (var boid in Manager.Instance.boid) {
                Vector2 diff = boid.transform.position - transform.position;
                float distance = diff.magnitude;
                
                if (distance <= alignmentRadius) {
                    alignmentCount++;
                    alignmentSteering += boid.velocity;
                }
                if (distance <= separationRadius) {
                    separationCount++;
                    separationSteering -= diff;
                }
                if (distance <= cohesionRadius) {
                    cohesionCount++;
                    cohesionSteering += (Vector2)boid.transform.position;
                }
            }
            separationSteering = Seek(separationSteering) * separationStrength;
            cohesionSteering = Seek(cohesionSteering / cohesionCount) * cohesionStrength;
            alignmentSteering = Seek(alignmentSteering / alignmentCount) * alignmentStrength;
            velocity += separationSteering + cohesionSteering + alignmentSteering;
            ProcessMovement();
        }
    }
}
