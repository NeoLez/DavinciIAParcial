using StateMachine;
using UnityEngine;
using Utils;

namespace Boids.HunterBehaviours {
    public class RestState : IState<HunterStates> {
        private readonly StateMachine<HunterStates> _stateMachine;
        private readonly Hunter _hunter;
        private readonly Timer _recoveryTimer;
        private readonly float _timeToRecovery;

        public RestState(StateMachine<HunterStates> stateMachine, Hunter hunter, float timeToRecovery) {
            _stateMachine = stateMachine;
            _hunter = hunter;
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
                _stateMachine.ChangeState(HunterStates.Patrol);
            }
        }
    }
}