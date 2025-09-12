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
        
        public float fov {
            get => _fov;
            protected set => _fov = value;
        }
        public float alignmentRadius {
            get => _alignmentRadius;
            protected set => _alignmentRadius = value;
        }
        public float alignmentStrength {
            get => _alignmentStrength;
            protected set => _alignmentStrength = value;
        }
        public float separationRadius {
            get => _separationRadius;
            protected set => _separationRadius = value;
        }
        public float separationStrength {
            get => _separationStrength;
            protected set => _separationStrength = value;
        }
        public float cohesionRadius {
            get => _cohesionRadius;
            protected set => _cohesionRadius = value;
        }
        public float cohesionStrength {
            get => _cohesionStrength;
            protected set => _cohesionStrength = value;
        }
        public float randomStrength {
            get => _randomStrength;
            protected set => _randomStrength = value;
        }
        public float foodDetectionRange {
            get => _foodDetectionRange;
            protected set => _foodDetectionRange = value;
        }
        public float hunterDetectionRange {
            get => _hunterDetectionRange;
            protected set => _hunterDetectionRange = value;
        }
        public float circleBoundRadius {
            get => _circleBoundRadius;
            protected set => _circleBoundRadius = value;
        }
        public float circleBoundStrength {
            get => _circleBoundStrength;
            protected set => _circleBoundStrength = value;
        }
        public bool highPerformanceMode {
            get => _highPerformanceMode;
            protected set => _highPerformanceMode = value;
        }
    }
}