using UnityEngine;

namespace Boids.SO {
    [CreateAssetMenu(menuName = "SO/Boid")]
    public class BoidSO : MovementAgentSO {
        [Header("Flocking Options")]
        [SerializeField] private float _fov;
        [SerializeField] private float _alignmentRadius;
        [Range(0,1)] [SerializeField] private float _alignmentStrength;
        [SerializeField] private float _separationRadius;
        [Range(0,1)] [SerializeField] private float _separationStrength;
        [SerializeField] private float _cohesionRadius;
        [Range(0,1)] [SerializeField] private float _cohesionStrength;
        
        [Range(0,1)] [SerializeField] private float _randomStrength;
        
        [Header("Detection Ranges")]
        [SerializeField] private float _foodDetectionRange;
        [SerializeField] private float _hunterDetectionRange;
        

        [Header("Bounds")]
        [SerializeField] private float _circleBoundRadius;
        [SerializeField] private float _circleBoundStrength;

        [Header("Performance")]
        [SerializeField] private bool _highPerformanceMode;
        
        public float fov  => _fov;
        public float alignmentRadius => _alignmentRadius;
        public float alignmentStrength => _alignmentStrength;
        public float separationRadius => _separationRadius;
        public float separationStrength => _separationStrength;
        public float cohesionRadius => _cohesionRadius;
        public float cohesionStrength => _cohesionStrength;
        public float randomStrength => _randomStrength;
        public float foodDetectionRange => _foodDetectionRange;
        public float hunterDetectionRange => _hunterDetectionRange;
        public float circleBoundRadius => _circleBoundRadius;
        public float circleBoundStrength => _circleBoundStrength;
        public bool highPerformanceMode => _highPerformanceMode;
    }
}