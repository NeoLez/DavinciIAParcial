using StateMachine;
using UnityEngine;
using Utils;

namespace Boids.HunterBehaviours {
    public class RestState : IState<HunterStates> {
        private StateMachine<HunterStates> _stateMachine;
        private Hunter _hunter;
        private Timer _recoveryTimer;
        private float _timeToRecovery;

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
            _hunter.StopMoving();
            if (_recoveryTimer.HasFinished()) {
                _hunter.RestoreEnergy();
                _stateMachine.ChangeState(HunterStates.Patrol);
            }
        }
    }
}