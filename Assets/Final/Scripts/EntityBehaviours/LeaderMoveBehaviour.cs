using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Final.Scripts.EntityBehaviours
{
    public class LeaderMoveBehaviour : IState<LeaderBehaviours>
    {
        private Leader _leader;
        private Stack<Point> _path = new();
        private Point _goal;
        private StateMachine<LeaderBehaviours> _stateMachine;
        private float startAttackCooldown;

        public LeaderMoveBehaviour(StateMachine<LeaderBehaviours> stateMachine, Leader leader, Point goal)
        {
            _stateMachine = stateMachine;
            _leader = leader;
            _goal = goal;
        }
        
        public void OnEnter()
        {
            Debug.Log(_leader.name + " has started moving ");
            startAttackCooldown = Time.time + _leader.settings.OnMoveAttackCooldown;
            StartPath();
        }

        public void OnExit()
        {
            _path.Clear();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_leader.GetHealth() <= _leader.settings.FleeHealth)
            {
                _stateMachine.ChangeState(LeaderBehaviours.Flee);
                return;
            }

            //Debug.Log(_leader.name + " " + _leader.GetEnemiesInLOS().Count);
            if (Time.time >= startAttackCooldown && _leader.AreEnemiesInLOS())
            {
                //Debug.Log(_leader.gameObject.name);
                _stateMachine.ChangeState(LeaderBehaviours.Attack);
                return;
            }
            
            
            
            if (Input.GetMouseButtonDown(_leader.settings.MouseClickCommand))
            {
                StartPath();
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
        
        private void StartPath()
        {
            startAttackCooldown = Time.time + _leader.settings.OnMoveAttackCooldown;
            _goal.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PointManager.Instance.UpdatePoint(_goal);
            PointManager.Instance.UpdatePoint(_leader._point);
            _path = PointManager.Instance.CalculatePath(_leader._point, _goal);
            if (_path == null || _path.Count <= 1) return;
            _path.Pop();
        }
    }
}