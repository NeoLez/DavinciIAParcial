using System;
using UnityEngine;

namespace Boids {
    public class MovementAgent : MonoBehaviour {
        protected Vector2 velocity; //CHANGING THIS TO A PROPERTY MAKES EVERYTHING EXPLODE :3
        public Vector2 GetVelocity() => velocity;
        [SerializeField] protected float maxSpeed;
        [SerializeField] protected float maxTurnSpeed;
        
        protected void ProcessMovement() {
            if (velocity.magnitude > maxSpeed) {
                velocity.Normalize();
                velocity *= maxSpeed;
            }
            
            transform.rotation = Quaternion.Euler(0, 0, (float)(Math.Atan2(velocity.y, velocity.x) / Math.PI * 180));

            transform.position = (Vector2)transform.position + velocity * Time.deltaTime;
        }
        
        public Vector2 Seek(Vector2 target) {
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

        public Vector2 Arrive(Vector2 target, float radius) {
            Vector2 difference = target - (Vector2)transform.position;
            float distance = difference.magnitude;

            if (distance > radius) return Seek(target);

            Vector2 desiredVelocity;
            if(radius == 0)
                desiredVelocity = difference.normalized * maxSpeed;
            else
                desiredVelocity = difference.normalized * (maxSpeed * distance / radius);
            
            Vector2 steering = desiredVelocity - velocity;
            steering = Vector3.ClampMagnitude(steering, maxTurnSpeed);
            return steering;
        }
        
        public Vector2 Flee(Vector2 target) {
            return -Seek(target);
        }
        public Vector2 Pursue(Vector2 target, Vector2 targetSpeed, float predictionTime) {
            return Seek(target + targetSpeed * predictionTime);
        }
        public Vector2 Evade(Vector2 target, Vector2 targetSpeed, float predictionTime) {
            return -Pursue(target, targetSpeed, predictionTime);
        }

        public void AddVelocity(Vector2 velocityToAdd) {
            velocity += velocityToAdd;
        }

        public float GetMaxSpeed() {
            return maxSpeed;
        }

        public float GetMaxTurnSpeed() {
            return maxTurnSpeed;
        }
    }
}