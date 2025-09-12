using UnityEngine;

namespace Boids.SO {
    public class MovementAgentSO : ScriptableObject {
        [Header("Movement")]
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _maxTurnSpeed;
        
        public float maxSpeed => _maxSpeed;
        public float maxTurnSpeed => _maxTurnSpeed;
    }
}