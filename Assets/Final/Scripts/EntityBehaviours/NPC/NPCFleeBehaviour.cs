using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Final.Scripts.EntityBehaviours {
    public class NPCFleeBehaviour : IState<NPCBehaviours>{
        private NPC _npc;
        private Stack<Point> _path = new();
        private StateMachine<NPCBehaviours> _stateMachine;

        public NPCFleeBehaviour(StateMachine<NPCBehaviours> stateMachine, NPC npc)
        {
            _stateMachine = stateMachine;
            _npc = npc;
        }

        public void OnEnter() {
            _npc.SetColor(Color.blue);
            StartPath(_npc.team.teamBase);
        }

        public void OnExit() {
            _path.Clear();
        }

        public void OnUpdate(float deltaTime) {
            if (_npc.GetHealth() == _npc.settings.MaxHealth) {
                _stateMachine.ChangeState(NPCBehaviours.Idle);
                return;
            }
            
            if (_path == null || _path.Count == 0)
            {
                return;
            };
            
            if (!PointManager.Instance.ArePointsInView(_path.Peek(), _npc._point)) {
                StartPath(_npc.team.teamBase);
                return;
            }
            
            Vector2 distanceVector = _path.Peek().transform.position - _npc.transform.position;
            float distance = distanceVector.magnitude;
            Vector2 direction = distanceVector / distance;
            float maxDistanceThisFrame = _npc.settings.Speed * Time.deltaTime;

            if (distance <= maxDistanceThisFrame)
            {
                _path.Pop();
            }

            _npc.velocity += direction * _npc.settings.VelocityMove;
        }
        
        private void StartPath(Point goal)
        {
            PointManager.Instance.UpdatePoint(goal);
            PointManager.Instance.UpdatePoint(_npc._point);
            _path = PointManager.Instance.CalculatePath(_npc._point, goal);
            if (_path == null || _path.Count <= 1) return;
            
            _path.Pop();
        }
    }
}