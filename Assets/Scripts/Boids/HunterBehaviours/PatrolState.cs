using System;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Boids.HunterBehaviours {
    public class PatrolState : IState<HunterStates> {
        private readonly StateMachine<HunterStates> _stateMachine;
        private readonly Hunter _hunter;
        private readonly List<Target> _patrolPositions;
        private readonly float _targetMinimumDistance;
        private readonly float _viewRange;
        private int _currentTargetIndex;
        
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
                float distance = Vector2.Distance(_hunter.transform.position, patrolPosition.transform.position);
                if (distance < minDistance) {
                    _currentTargetIndex = i;
                    minDistance = distance;
                }
            }
        }

        public void OnExit() {
        }

        public void OnUpdate(float deltaTime) {
            if (HandleBoidSight()) return;
            
            HandlePatrol();
            _hunter.AddEnergy(deltaTime);
        }

        /// <summary>
        /// If there is a <c>Boid</c> in sight, switch the state to Hunting.
        /// </summary>
        /// <returns>True if a <c>boid</c> was found. Otherwise, returns false.</returns>
        private bool HandleBoidSight() {
            foreach (var boid in Manager.instance.Boids) {
                float distance = Vector2.Distance(_hunter.transform.position, boid.transform.position);
                if (distance <= _viewRange) {
                    _stateMachine.ChangeState(HunterStates.Hunting);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles patrolling movement and switching of targets if a patrol point is reached.
        /// </summary>
        private void HandlePatrol() {
            float distance = Vector2.Distance(_patrolPositions[_currentTargetIndex].transform.position, _hunter.transform.position);
            if (distance <= _targetMinimumDistance) {
                _currentTargetIndex = (_currentTargetIndex + 1) % _patrolPositions.Count;
            }

            Target target = _patrolPositions[_currentTargetIndex];
            _hunter.AddVelocity(_hunter.Arrive(target.transform.position, target.radius));
        }
    }
}