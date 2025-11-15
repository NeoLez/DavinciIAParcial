using System.Collections.Generic;
using Parcial_2.Scripts.EnemyBehaviours;
using StateMachine;
using UnityEngine;

namespace Parcial_2.Scripts {
    public class Enemy : MonoBehaviour{
        [SerializeField] [Range(0, 50)] public float soundDetectionRadius = 2;
        [SerializeField] [Range(0, 50)] public float viewDetectionRadius = 10;
        [SerializeField] [Range(0, 2 * Mathf.PI)] public float viewDetectionAngle = 60;
        [SerializeField] [Range(-Mathf.PI, Mathf.PI)] public double viewDetectionAngleOffset;
        [SerializeField] private List<Node> path= new();
        public float speed;
        public float killDistance;
        
        public Node _node { get; private set; }
        private NodeManager _nodeManager;
        private StateMachine<EnemyBehaviour> _stateMachine;

        private void Start() {
            _node = GetComponent<Node>();
            _nodeManager = NodeManager.Instance;
            _stateMachine = new();
            PatrolState patrolState = new(_stateMachine, path, this);
            _stateMachine.AddState(EnemyBehaviour.Patrol, patrolState);
            ChaseState chaseState = new(_stateMachine, this, killDistance);
            _stateMachine.AddState(EnemyBehaviour.Chase, chaseState);
            _stateMachine.ChangeState(EnemyBehaviour.Patrol);
        }

        private void Update() {
            if (IsPointInViewRadius(_nodeManager.player.transform.position)) {
                _nodeManager.enemyTarget.transform.position = _nodeManager.player.transform.position;
                _nodeManager.AddNode(_nodeManager.enemyTarget);
                _nodeManager.UpdateNode(_nodeManager.enemyTarget);
            }
            _nodeManager.UpdateNode(_node);
            
            _stateMachine.UpdateState(Time.deltaTime);
        }

        private bool IsPointInViewRadius(Vector3 pos) {
            Vector2 vectorToPoint = pos - transform.position;
            if (vectorToPoint.magnitude <= soundDetectionRadius) return true;
            
            if (vectorToPoint.magnitude <= viewDetectionRadius) {
                double angle = vectorToPoint.GetAngle();
                double offsetAngle = MathUtils.ATan2AngleToNormal(viewDetectionAngleOffset);
                double rightLimitAngle = MathUtils.NormalizeAngle(offsetAngle - viewDetectionAngle/2);
                double leftLimitAngle = MathUtils.NormalizeAngle(offsetAngle + viewDetectionAngle/2);
				
                if (leftLimitAngle > rightLimitAngle) {
                    return angle >= rightLimitAngle && angle <= leftLimitAngle;
                }
				
                return !(angle <= rightLimitAngle && angle >= leftLimitAngle);
            }
			
            return false;
        }
        
        
        
        
        [SerializeField] private bool showViewCone;
        [SerializeField] private bool showSoundDetectionSphere;
        [SerializeField] [Range(3, 100)] private int gizmoResolution = 3;
        private void OnDrawGizmos() {
            if (showViewCone) {
                Vector3[] points = new Vector3[gizmoResolution];
                points[0] = transform.position;
                for (int i = 1; i <= points.Length - 1; i++) {
                    float angle = (i - 1) * viewDetectionAngle / (gizmoResolution - 2);
                    points[i] = transform.position + new Vector3(Mathf.Cos(angle + (float)viewDetectionAngleOffset - viewDetectionAngle / 2), Mathf.Sin(angle + (float)viewDetectionAngleOffset - viewDetectionAngle / 2), 0) * viewDetectionRadius;
                }
                Gizmos.DrawLineStrip(points, true);
            }
            if (showSoundDetectionSphere) {
                Gizmos.DrawWireSphere(transform.position, soundDetectionRadius);
            }
        }
    }
}