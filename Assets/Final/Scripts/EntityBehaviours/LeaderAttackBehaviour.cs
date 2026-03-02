using StateMachine;
using UnityEngine;

namespace Final.Scripts.EntityBehaviours
{
    public class LeaderAttackBehaviour : IState<LeaderBehaviours>
    {
        private Leader _leader;
        private StateMachine<LeaderBehaviours> _stateMachine;
        private IEntity closestEnemy;
        private float distance;
        private float nextAttackTime;
        
        public LeaderAttackBehaviour(StateMachine<LeaderBehaviours> stateMachine, Leader leader)
        {
            _leader = leader;
            _stateMachine = stateMachine;
        }
        
        public void OnEnter()
        {
            closestEnemy = _leader.GetNearestEnemiesInLOS();
            _leader.SetColor(Color.red);
        }

        public void OnExit()
        {
            
        }

        public void OnUpdate(float deltaTime)
        {
            if (_leader.GetHealth() <= _leader.settings.FleeHealth)
            {
                _stateMachine.ChangeState(LeaderBehaviours.Flee);
                return;
            }
            
            if (Input.GetMouseButtonDown(_leader.settings.MouseClickCommand))
            {
                closestEnemy = null;
                _leader._goal.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _stateMachine.ChangeState(LeaderBehaviours.Move);
                return;
            }

            if (closestEnemy == null || !_leader.IsPointInLOS(closestEnemy.GetGameObject().transform.position))
            {
                closestEnemy = _leader.GetNearestEnemiesInLOS();
                
                if (closestEnemy == null)
                {
                    _stateMachine.ChangeState(LeaderBehaviours.Move);
                    return;
                }
            }

            Vector2 differenceVector = closestEnemy.GetGameObject().transform.position - _leader.transform.position;
            distance = differenceVector.magnitude;
            differenceVector /= distance;
            
            float maxDistanceThisFrame = _leader.settings.Speed * Time.deltaTime;
            float distanceNextFrame = distance - maxDistanceThisFrame;
            if (distanceNextFrame < closestEnemy.GetSize())
            {
                maxDistanceThisFrame += distanceNextFrame - closestEnemy.GetSize();
            }
            
            
            _leader.transform.position += (Vector3)(differenceVector * maxDistanceThisFrame);
            
            _leader._viewDetectionAngleOffset = Mathf.Atan2(differenceVector.y,differenceVector.x);


            if (nextAttackTime <= Time.time && distance <= _leader.settings.AttackDistance)
            {
                closestEnemy.TakeDamage(_leader.settings.AttackDamage);
                nextAttackTime = Time.time + _leader.settings.AttackCooldown;
            }
        }
    }
}