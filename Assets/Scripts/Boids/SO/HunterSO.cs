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
        public float timeToRecovery => _timeToRecovery;
        public float minimumPatrolPointDistance => _minimumPatrolPointDistance;
        public float viewDistance => _viewDistance;
        public float stopSlowdownSpeed => _stopSlowdownSpeed;
        public float boidEatDistance => _boidEatDistance;
        public float maxEnergy => _maxEnergy;
    }
}