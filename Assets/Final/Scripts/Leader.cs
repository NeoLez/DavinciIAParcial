using System;
using System.Collections.Generic;
using Final.Scripts.EntityBehaviours;
using Final.Scripts.EntitySettings;
using Parcial_2.Scripts;
using StateMachine;
using UnityEngine;

namespace Final.Scripts
{
    public class Leader : MonoBehaviour, IEntity
    {
        [SerializeField] public LeaderSettingsSO settings;
        [SerializeField] [Range(-Mathf.PI, Mathf.PI)] public double _viewDetectionAngleOffset;
        
        public Point _point { get; private set; }
        [SerializeField] public Point _goal;
        private PointManager _pointManager;
        private StateMachine<LeaderBehaviours> _stateMachine;
        [SerializeField] private int health;
        [SerializeField] private SpriteRenderer sprite;
        public Team team;
        public Action<Vector2> OnGotHit;
        
        private void Start() {
            _point = GetComponent<Point>();
            health = settings.MaxHealth;
            _pointManager = PointManager.Instance;
            _stateMachine = new();
            _stateMachine.AddState(LeaderBehaviours.Move, new LeaderMoveBehaviour(_stateMachine, this, _goal));
            _stateMachine.AddState(LeaderBehaviours.Attack, new LeaderAttackBehaviour(_stateMachine, this));
            _stateMachine.AddState(LeaderBehaviours.Flee, new LeaderFleeBehaviour(_stateMachine, this));
            _stateMachine.AddState(LeaderBehaviours.Idle, new LeaderIdleBehaviour(_stateMachine, this));
            _stateMachine.ChangeState(LeaderBehaviours.Idle);
        }

        private void Update()
        {
            _stateMachine.UpdateState(Time.deltaTime);
        }

        public void TakeDamage(int dmg)
        {
            health -= dmg;
            
            if (health > settings.MaxHealth) health = settings.MaxHealth;
            if (health <= 0)
            {
                Debug.Log("Death "+name);
                PointManager.Instance.RemovePoint(GetComponent<Point>());
                PointManager.Instance.RemovePoint(_goal);
                Destroy(_goal.gameObject);
                Destroy(gameObject);
            }
        }

        public int GetHealth() => health;
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public float GetSize()
        {
            return settings.Size;
        }

        public Team GetTeam() {
            return team;
        }

        public void SetColor(Color color) {
            sprite.color = color;
        }

        public List<IEntity> GetEnemiesInLOS()
        {
            List<IEntity> res = new();
            var entities = Physics2D.OverlapCircleAll(transform.position, settings.ViewDetectionRadius, settings.EntityLayer);
            foreach (var entity in entities)
            {
                if (entity.gameObject == gameObject) continue;
                if (entity.gameObject.GetComponent<IEntity>().GetTeam() == team) continue;
                if (IsPointInLOS(entity.transform.position))
                {
                    res.Add(entity.gameObject.GetComponent<IEntity>());
                }
            }
            return res;
        }
        
        public IEntity GetNearestEnemiesInLOS()
        {
            IEntity closestEnemy = null;
            float minimumEnemyDistance = float.MaxValue;
            var entities = Physics2D.OverlapCircleAll(transform.position, settings.ViewDetectionRadius, settings.EntityLayer);
            
            foreach (var entity in entities)
            {
                if (entity.gameObject == gameObject) continue;
                if (entity.gameObject.GetComponent<IEntity>().GetTeam() == team) continue;
                if (IsPointInLOS(entity.transform.position))
                {
                    float distance = Vector2.Distance(entity.transform.position, transform.position);
                    if (distance < minimumEnemyDistance)
                    {
                        minimumEnemyDistance = distance;
                        closestEnemy = entity.GetComponent<IEntity>();
                    }
                }
            }

            return closestEnemy;
        }
        
        public bool AreEnemiesInLOS()
        {
            var entities = Physics2D.OverlapCircleAll(transform.position, settings.ViewDetectionRadius, settings.EntityLayer);
            foreach (var entity in entities)
            {
                if (entity.gameObject == gameObject) continue;
                if (entity.gameObject.GetComponent<IEntity>().GetTeam() == team) continue;
                if (IsPointInLOS(entity.transform.position))
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool IsPointInLOS(Vector2 position)
        {
            if (IsPointInViewRadius(position)) {
                Vector2 direction = position - (Vector2)transform.position;
                if (!Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, PointManager.Instance.layerMask))
                {
                    return true;
                } 
            }

            return false;
        } 
        
        public bool IsPointInViewRadius(Vector3 pos) {
            Vector2 vectorToPoint = pos - transform.position;
            if (vectorToPoint.magnitude <= settings.SoundDetectionRadius) return true;
            
            if (vectorToPoint.magnitude <= settings.ViewDetectionRadius) {
                double angle = vectorToPoint.GetAngle();
                double offsetAngle = MathUtils.ATan2AngleToNormal(_viewDetectionAngleOffset);
                double rightLimitAngle = MathUtils.NormalizeAngle(offsetAngle - settings.ViewDetectionAngle/2);
                double leftLimitAngle = MathUtils.NormalizeAngle(offsetAngle + settings.ViewDetectionAngle/2);
				
                if (leftLimitAngle > rightLimitAngle) {
                    return angle >= rightLimitAngle && angle <= leftLimitAngle;
                }
				
                return !(angle <= rightLimitAngle && angle >= leftLimitAngle);
            }
			
            return false;
        }
        
        
        private void OnDrawGizmos() {
            if (settings.ShowViewCone) {
                Vector3[] points = new Vector3[settings.GizmoResolution];
                points[0] = transform.position;
                for (int i = 1; i <= points.Length - 1; i++) {
                    float angle = (i - 1) * settings.ViewDetectionAngle / (settings.GizmoResolution - 2);
                    points[i] = transform.position + new Vector3(Mathf.Cos(angle + (float)_viewDetectionAngleOffset - settings.ViewDetectionAngle / 2), Mathf.Sin(angle + (float)_viewDetectionAngleOffset - settings.ViewDetectionAngle / 2), 0) * settings.ViewDetectionRadius;
                }
                Gizmos.DrawLineStrip(points, true);
            }
            if (settings.ShowSoundDetectionSphere) {
                Gizmos.DrawWireSphere(transform.position, settings.SoundDetectionRadius);
            }
        }
    }
}