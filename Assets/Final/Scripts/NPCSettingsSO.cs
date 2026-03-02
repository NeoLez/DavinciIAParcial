using UnityEngine;

namespace Final.Scripts {
    [CreateAssetMenu(menuName = "SO/NPC")]
    public class NPCSettingsSO : ScriptableObject {
        [SerializeField] [Range(0, 50)] private float _soundDetectionRadius = 2;
        [SerializeField] [Range(0, 50)] private float _viewDetectionRadius = 10;
        [SerializeField] [Range(0, 2 * Mathf.PI)] private float _viewDetectionAngle = 60;
        [SerializeField] private float _speed;
        [SerializeField] private float _attackDistance;
        [SerializeField] private bool _showViewCone;
        [SerializeField] private bool _showSoundDetectionSphere;
        [SerializeField] [Range(3, 100)] private int _gizmoResolution = 6;
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _fleeHealth;
        [SerializeField] private LayerMask _entityLayer;
        [SerializeField] private float _attackCooldown;
        [SerializeField] private int _attackDamage;
        [SerializeField] private float _onMoveAttackCooldown;
        [SerializeField] private float _size;
        [SerializeField] private float _leaderCloseAreaRadius;
        [SerializeField] private float _flockingAreaScanSize;
        [SerializeField] private float _velocityMove;
        [SerializeField] private float _velocitySeparation;
        [SerializeField] private float _separationRadius;

        public float SoundDetectionRadius => _soundDetectionRadius;
        public float ViewDetectionRadius => _viewDetectionRadius;
        public float ViewDetectionAngle => _viewDetectionAngle;
        public float Speed => _speed;
        public float AttackDistance => _attackDistance;
        public bool ShowViewCone => _showViewCone;
        public bool ShowSoundDetectionSphere => _showSoundDetectionSphere;
        public int GizmoResolution => _gizmoResolution;
        public int MaxHealth => _maxHealth;
        public int FleeHealth => _fleeHealth;
        public LayerMask EntityLayer => _entityLayer;
        public float AttackCooldown => _attackCooldown;
        public int AttackDamage => _attackDamage;
        public float OnMoveAttackCooldown => _onMoveAttackCooldown;
        public float Size => _size;
        public float LeaderCloseAreaRadius => _leaderCloseAreaRadius;
        public float FlockingAreaScanSize => _flockingAreaScanSize;
        public float VelocityMove => _velocityMove;
        public float VelocitySeparation => _velocitySeparation;
        public float SeparationRadius => _separationRadius;
    }
}