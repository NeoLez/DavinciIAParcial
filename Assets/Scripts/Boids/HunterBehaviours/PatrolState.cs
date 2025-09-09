using System;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Boids.HunterBehaviours {
    public class PatrolState : IState<HunterStates> {
        private StateMachine<HunterStates> _stateMachine;
        private Hunter _hunter;
        private List<Target> _patrolPositions;
        private int _currentTargetIndex;
        private float _targetMinimumDistance;
        private float _viewRange;
        
        public PatrolState(StateMachine<HunterStates> stateMachine, Hunter hunter, List<Target> patrolPositions, float targetMinimumDistance, float viewRange) {
            _stateMachine = stateMachine;
            _hunter = hunter;
            _patrolPositions = patrolPositions;
            _targetMinimumDistance = targetMinimumDistance;
            _viewRange = viewRange;
        }
        
        public void OnEnter() {
            Debug.Log("Patrol");
            float minDistance = Single.MaxValue;
            for (int i = 0; i < _patrolPositions.Count; i++) {
                Target patrolPosition = _patrolPositions[i];
                float distance = Vector2.Distance(_hunter.transform.position, patrolPosition.position);
                if (distance < minDistance) {
                    _currentTargetIndex = i;
                    minDistance = distance;
                }
            }
        }

        public void OnExit() {
        }

        public void OnUpdate(float _) {
            HandlePatrol();

            HandleBoidSight();
        }

        private void HandleBoidSight() {
            foreach (var boid in Manager.instance.Boids) {
                float distance = Vector2.Distance(_hunter.transform.position, boid.transform.position);
                if (distance <= _viewRange) {
                    _stateMachine.ChangeState(HunterStates.Hunting);
                    return;
                }
            }
        }

        private void HandlePatrol() {
            float distance = Vector2.Distance(_patrolPositions[_currentTargetIndex].position, _hunter.transform.position);
            if (distance <= _targetMinimumDistance) {
                _currentTargetIndex = (_currentTargetIndex + 1) % _patrolPositions.Count;
            }

            Target t = _patrolPositions[_currentTargetIndex];
            _hunter.Arrive(t.position, t.radius);
        }
    }
}