using UnityEngine;

namespace Boids.SO {
    [CreateAssetMenu(menuName = "SO/Hunter")]
    public class HunterSO : MovementAgentSO {
        [SerializeField] private float _stopSlowdownSpeed;
        [SerializeField] private float _timeToRecovery;
        [SerializeField] private float _minimumPatrolPointDistance;
        [SerializeField] private float _viewDistance;
        [SerializeField] private float _boidEatDistance;
        [SerializeField] private float _maxEnergy;
        public float timeToRecovery {
            get => _timeToRecovery;
            protected set => _timeToRecovery = value;
        }
        public float minimumPatrolPointDistance {
            get => _minimumPatrolPointDistance;
            protected set => _minimumPatrolPointDistance = value;
        }
        public float viewDistance {
            get => _viewDistance;
            protected set => _viewDistance = value;
        }
        public float stopSlowdownSpeed {
            get => _stopSlowdownSpeed;
            protected set => _stopSlowdownSpeed = value;
        }
        public float boidEatDistance {
            get => _boidEatDistance;
            protected set => _boidEatDistance = value;
        }
        public float maxEnergy {
            get => _maxEnergy;
            protected set => _maxEnergy = value;
        }
    }
}