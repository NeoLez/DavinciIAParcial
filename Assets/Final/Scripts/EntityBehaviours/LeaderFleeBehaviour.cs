using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Final.Scripts.EntityBehaviours
{
    public class LeaderFleeBehaviour : IState<LeaderBehaviours>
    {
        private StateMachine<LeaderBehaviours> _stateMachine;
        private Leader _leader;
        private Stack<Point> _path = new();

        public LeaderFleeBehaviour(StateMachine<LeaderBehaviours> stateMachine, Leader leader)
        {
            _stateMachine = stateMachine;
            _leader = leader;
        }
        
        public void OnEnter()
        {
            _leader.SetColor(Color.blue);
            StartPath(_leader.team.teamBase);
        }

        public void OnExit()
        {
            _path.Clear();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_leader.GetHealth() == _leader.settings.MaxHealth) {
                _stateMachine.ChangeState(LeaderBehaviours.Idle);
                return;
            }
            
            if (_path == null || _path.Count == 0)
            {
                return;
            };
            
            Vector2 distanceVector = _path.Peek().transform.position - _leader.transform.position;
            float distance = distanceVector.magnitude;
            Vector2 direction = distanceVector / distance;
            float maxDistanceThisFrame = _leader.settings.Speed * Time.deltaTime;

            if (distance <= maxDistanceThisFrame)
            {
                _path.Pop();
            }

            _leader.transform.position += (Vector3)(direction * maxDistanceThisFrame);
            _leader._viewDetectionAngleOffset = Mathf.Atan2(distanceVector.y,distanceVector.x);
        }
        
        private void StartPath(Point goal)
        {
            PointManager.Instance.UpdatePoint(goal);
            PointManager.Instance.UpdatePoint(_leader._point);
            _path = PointManager.Instance.CalculatePath(_leader._point, goal);
            if (_path == null || _path.Count <= 1) return;
            
            _path.Pop();
        }
    }
}