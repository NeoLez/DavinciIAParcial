using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Final.Scripts.EntityBehaviours {
    public class NPCAttackBehaviour : IState<NPCBehaviours>{
        private NPC _npc;
        private StateMachine<NPCBehaviours> _stateMachine;
        private IEntity closestEnemy;
        private float distance;
        private float nextAttackTime;

        public NPCAttackBehaviour(StateMachine<NPCBehaviours> stateMachine, NPC npc)
        {
            _stateMachine = stateMachine;
            _npc = npc;
        }

        public void OnEnter() {
            closestEnemy = _npc.GetNearestEnemiesInLOS();
            _npc.SetColor(Color.red);
        }

        public void OnExit() {
            
        }

        public void OnUpdate(float deltaTime) {
            if (_npc.GetHealth() <= _npc.settings.FleeHealth)
            {
                _stateMachine.ChangeState(NPCBehaviours.Flee);
                return;
            }
            
            if (_npc.team.leader != null  && Vector2.Distance(_npc.transform.position, _npc.team.leader.transform.position) > _npc.settings.LeaderCloseAreaRadius || !PointManager.Instance.ArePointsInView(_npc.team.leader._point, _npc._point))
            {
                closestEnemy = null;
                _stateMachine.ChangeState(NPCBehaviours.Move);
                return;
            }

            if (closestEnemy as Object == null || !_npc.IsPointInLOS(closestEnemy.GetGameObject().transform.position))
            {
                closestEnemy = _npc.GetNearestEnemiesInLOS();
                
                if (closestEnemy as Object == null)
                {
                    _stateMachine.ChangeState(NPCBehaviours.Move);
                    return;
                }
            }

            Vector2 differenceVector = closestEnemy.GetGameObject().transform.position - _npc.transform.position;
            distance = differenceVector.magnitude;
            differenceVector /= distance;
            
            float maxDistanceThisFrame = _npc.settings.Speed * Time.deltaTime;
            float distanceNextFrame = distance - maxDistanceThisFrame;
            if (distanceNextFrame < closestEnemy.GetSize())
            {
                maxDistanceThisFrame += distanceNextFrame - closestEnemy.GetSize();
            }
            
            _npc.velocity += differenceVector * _npc.settings.VelocityMove;


            if (nextAttackTime <= Time.time && distance <= _npc.settings.AttackDistance)
            {
                closestEnemy.TakeDamage(_npc.settings.AttackDamage);
                nextAttackTime = Time.time + _npc.settings.AttackCooldown;
            }
        }
    }
}