using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Final.Scripts.EntityBehaviours {
    public class NPCMoveBehaviour : IState<NPCBehaviours>{
        private NPC _npc;
        private Stack<Point> _path = new();
        private StateMachine<NPCBehaviours> _stateMachine;
        private float startAttackCooldown;

        public NPCMoveBehaviour(StateMachine<NPCBehaviours> stateMachine, NPC npc)
        {
            _stateMachine = stateMachine;
            _npc = npc;
        }

        public void OnEnter() {
            _npc.SetColor(Color.green);
            startAttackCooldown = Time.time + _npc.settings.OnMoveAttackCooldown;
            StartPath();
        }

        public void OnExit() {
            _path.Clear();
        }

        public void OnUpdate(float deltaTime) {
            if (_npc.GetHealth() <= _npc.settings.FleeHealth)
            {
                _stateMachine.ChangeState(NPCBehaviours.Flee);
                return;
            }

            
            if (Time.time >= startAttackCooldown && _npc.AreEnemiesInLOS())
            {
                _stateMachine.ChangeState(NPCBehaviours.Attack);
                return;
            }
            
            if (_path == null || _path.Count == 0)
            {
                _stateMachine.ChangeState(NPCBehaviours.Idle);
                return;
            }

            if (!PointManager.Instance.ArePointsInView(_path.Peek(), _npc._point)) {
                StartPath();
                return;
            }
            
            Vector2 distanceVector = _path.Peek().transform.position - _npc.transform.position;
            float distance = distanceVector.magnitude;
            Vector2 direction = distanceVector / distance;
            float maxDistanceThisFrame = _npc.settings.Speed * Time.deltaTime;


            _npc.velocity += _npc.Separation();
            _npc.velocity += direction * _npc.settings.VelocityMove;
            
            if (distance <= _npc.velocity.magnitude * Time.deltaTime)
            {
                _path.Pop();
            }
        }
        
        private void StartPath() {
            if (_npc.team.leader == null) {
                _stateMachine.ChangeState(NPCBehaviours.Idle);
                return;
            }
            var _goal = _npc.team.leader._point;
            startAttackCooldown = Time.time + _npc.settings.OnMoveAttackCooldown;
            PointManager.Instance.UpdatePoint(_goal);
            PointManager.Instance.UpdatePoint(_npc._point);
            _path = PointManager.Instance.CalculatePath(_npc._point, _goal);
            if (_path == null || _path.Count <= 1) return;
            _path.Pop();
        }
    }
}