using System;
using System.Collections.Generic;
using Parcial_2.Scripts;
using UnityEngine;
using StateMachine;

namespace Final.Scripts.EntityBehaviours {
    public class NPC : MonoBehaviour, IEntity {
        [SerializeField] public NPCSettingsSO settings;
        [SerializeField] [Range(-Mathf.PI, Mathf.PI)] public double _viewDetectionAngleOffset;
        
        public Point _point { get; private set; }
        private PointManager _pointManager;
        private StateMachine<NPCBehaviours> _stateMachine;
        [SerializeField] private int health;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private LayerMask entityLayerMask;
        [SerializeField] private LayerMask wallLayerMask;
        public Team team;
        
        private void Start() {
            _point = GetComponent<Point>();
            GetComponent<CircleCollider2D>().radius = settings.Size;
            health = settings.MaxHealth;
            _pointManager = PointManager.Instance;
            _stateMachine = new();
            _stateMachine.AddState(NPCBehaviours.Move, new NPCMoveBehaviour(_stateMachine, this));
            _stateMachine.AddState(NPCBehaviours.Idle, new NPCIdleBehaviour(_stateMachine, this));
            _stateMachine.AddState(NPCBehaviours.Attack, new NPCAttackBehaviour(_stateMachine, this));
            _stateMachine.AddState(NPCBehaviours.Flee, new NPCFleeBehaviour(_stateMachine, this));
            _stateMachine.ChangeState(NPCBehaviours.Idle);
        }
        
        private void Update()
        {
            _stateMachine.UpdateState(Time.deltaTime);
            ProcessMovement();
        }

        public Vector2 Separation() {
            Vector2 res = Vector2.zero;
            int total = 0;

            var hits = Physics2D.OverlapCircleAll(transform.position, settings.FlockingAreaScanSize, entityLayerMask);
            foreach (var hit in hits) {
                if (hit.gameObject == gameObject) continue;

                var entity = hit.GetComponent<IEntity>();
                var go = entity.GetGameObject();
                var distance = Vector2.Distance(transform.position, go.transform.position);
                
                if (distance < settings.SeparationRadius) {
                    res += (Vector2)((1 / (distance/settings.SeparationRadius) - 1) * settings.VelocitySeparation * (transform.position - go.transform.position).normalized);
                }

                total++;
            }
            
            return total == 0 ? Vector2.zero : res / total;
        }
        
        public Vector2 velocity;
        protected void ProcessMovement() {
            if (velocity.magnitude > settings.Speed) {
                velocity.Normalize();
                velocity *= settings.Speed;
            }
            
            _viewDetectionAngleOffset = Mathf.Atan2(velocity.y, velocity.x);
            var hit = Physics2D.Raycast(transform.position, velocity.normalized, velocity.magnitude * Time.deltaTime, wallLayerMask);
            var distance = hit.collider == null ? Time.deltaTime : hit.distance;
            transform.position += (Vector3)(velocity * Time.deltaTime);
            velocity *= 0.3f;
        }

        public void TakeDamage(int dmg)
        {
            health -= dmg;
            if (health > settings.MaxHealth) health = settings.MaxHealth;
            if (health <= 0)
            {
                Debug.Log("Death "+name);
                PointManager.Instance.RemovePoint(GetComponent<Point>());
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