using DefaultNamespace.Utils;
using Hunter;
using UnityEngine;

namespace StateMachine {
    public class RestState : IState<HunterStates> {
        private StateMachine<HunterStates> _stateMachine;
        private Boids.Hunter _hunter;
        private Timer recoveryTimer;
        private float _timeToRecovery;

        public RestState(StateMachine<HunterStates> stateMachine, Boids.Hunter hunter, float timeToRecovery) {
            _stateMachine = stateMachine;
            _hunter = hunter;
            recoveryTimer = new Timer();
            _timeToRecovery = timeToRecovery;
        }
        
        public void OnEnter() {
            Debug.Log("Rest");
            recoveryTimer.Start(_timeToRecovery);
        }

        public void OnExit() {
        }

        public void OnUpdate(float _) {
            _hunter.StopMoving();
            if (recoveryTimer.hasFinished()) {
                _hunter.RestoreEnergy();
                _stateMachine.ChangeState(HunterStates.Patrol);
            }
        }
    }
}