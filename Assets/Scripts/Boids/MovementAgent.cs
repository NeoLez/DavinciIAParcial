using System;
using Boids.SO;
using UnityEngine;

namespace Boids {
    public abstract class MovementAgent <T> : MonoBehaviour where T : MovementAgentSO{
        protected Vector2 velocity; //CHANGING THIS TO A PROPERTY MAKES EVERYTHING EXPLODE :3
        public Vector2 GetVelocity() => velocity;

        [NonSerialized] public T Settings;
        
        /// <summary>
        /// Initializes the <c>MovementAgent</c> to use the settings ScriptableObject provided as a parameter.
        /// </summary>
        /// <param name="settings">Settings ScriptableObject reference</param>
        public void Initialize(T settings) {
            Settings = settings;
        }
        
        /// <summary>
        /// Processes the movement of the agent and keeps its rotation aligned with its velocity.
        /// </summary>
        protected void ProcessMovement() {
            if (velocity.magnitude > Settings.maxSpeed) {
                velocity.Normalize();
                velocity *= Settings.maxSpeed;
            }
            
            transform.rotation = Quaternion.Euler(0, 0, (float)(Math.Atan2(velocity.y, velocity.x) / Math.PI * 180));

            transform.position = (Vector2)transform.position + velocity * Time.deltaTime;
        }
        
        public Vector2 Seek(Vector2 target) {
            Vector2 desiredVelocity = target - (Vector2)transform.position;
            desiredVelocity.Normalize();
            desiredVelocity *= Settings.maxSpeed;

            Vector2 turnVector = desiredVelocity - velocity;
            if (turnVector.magnitude > Settings.maxTurnSpeed) {
                turnVector.Normalize();
                turnVector *= Settings.maxTurnSpeed;
            }
            
            return turnVector;
        }

        public Vector2 Arrive(Vector2 target, float radius) {
            Vector2 difference = target - (Vector2)transform.position;
            float distance = difference.magnitude;

            if (distance > radius) return Seek(target);

            Vector2 desiredVelocity;
            if(radius == 0)
                desiredVelocity = difference.normalized * Settings.maxSpeed;
            else
                desiredVelocity = difference.normalized * (Settings.maxSpeed * distance / radius);
            
            Vector2 steering = desiredVelocity - velocity;
            steering = Vector3.ClampMagnitude(steering, Settings.maxTurnSpeed);
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
            return Settings.maxSpeed;
        }
    }
}