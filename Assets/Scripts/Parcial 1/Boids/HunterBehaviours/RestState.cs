using StateMachine;
using UnityEngine;
using Utils;

namespace Boids.HunterBehaviours {
    public class RestState : IState<HunterStates> {
        private readonly StateMachine<HunterStates> _stateMachine;
        private readonly Hunter _hunter;
        private readonly float _viewRange;
        private readonly Timer _recoveryTimer;
        private readonly float _timeToRecovery;

        public RestState(StateMachine<HunterStates> stateMachine, Hunter hunter, float timeToRecovery, float viewRange) {
            _stateMachine = stateMachine;
            _hunter = hunter;
            _viewRange = viewRange;
            _recoveryTimer = new Timer();
            _timeToRecovery = timeToRecovery;
        }
        
        public void OnEnter() {
            Debug.Log("Rest");
            _recoveryTimer.Start(_timeToRecovery);
        }

        public void OnExit() {
        }

        public void OnUpdate(float _) {
            _hunter.SlowDown();
            if (_recoveryTimer.HasFinished()) {
                _hunter.RestoreEnergy();
                if(HandleBoidSight()) return;
                
                _stateMachine.ChangeState(HunterStates.Patrol);
            }
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
    }
}