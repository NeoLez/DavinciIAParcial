using System;
using UnityEngine;

namespace Boids {
    public class MovementAgent : MonoBehaviour {
        [SerializeField] public Vector2 velocity;
        [SerializeField] public float maxSpeed;
        [SerializeField] public float maxTurnSpeed;
        
        protected void ProcessMovement() {
            if (velocity.magnitude > maxSpeed) {
                velocity.Normalize();
                velocity *= maxSpeed;
            }
            
            transform.rotation = Quaternion.Euler(0, 0, (float)(Math.Atan2(velocity.y, velocity.x) / Math.PI * 180));

            transform.position = (Vector2)transform.position + velocity * Time.deltaTime;
        }
        
        protected Vector2 Seek(Vector2 target) {
            Vector2 desiredVelocity = target - (Vector2)transform.position;
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;

            Vector2 turnVector = desiredVelocity - velocity;
            if (turnVector.magnitude > maxTurnSpeed) {
                turnVector.Normalize();
                turnVector *= maxTurnSpeed;
            }
            
            return turnVector;
        }

        protected Vector2 Arrive(Vector2 target, float radius) {
            Vector2 difference = target - (Vector2)transform.position;
            float distance = difference.magnitude;

            if (distance > radius) return Seek(target);

            Vector2 desiredVelocity = difference.normalized * maxSpeed * (distance / radius);
            Vector2 steering = desiredVelocity - velocity;
            steering = Vector3.ClampMagnitude(steering, maxTurnSpeed);
            return steering;
        }
        
        protected Vector2 Flee(Vector2 target) {
            return -Seek(target);
        }
        protected Vector2 Pursue(Vector2 target, Vector2 targetSpeed, float predictionTime) {
            return Seek(target + targetSpeed * predictionTime);
        }
        protected Vector2 Evade(Vector2 target, Vector2 targetSpeed, float predictionTime) {
            return -Pursue(target, targetSpeed, predictionTime);
        }
    }
}