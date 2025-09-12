using UnityEngine;

namespace Boids.SO {
    public class MovementAgentSO : ScriptableObject {
        [Header("Movement")]
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _maxTurnSpeed;
        
        public float maxSpeed {
            get => _maxSpeed;
            protected set => _maxSpeed = value;
        }
        
        public float maxTurnSpeed {
            get => _maxTurnSpeed;
            protected set => _maxTurnSpeed = value;
        }
    }
}