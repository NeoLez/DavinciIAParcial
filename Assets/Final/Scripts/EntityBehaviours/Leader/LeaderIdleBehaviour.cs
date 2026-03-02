using StateMachine;
using UnityEngine;

namespace Final.Scripts.EntityBehaviours
{
    public class LeaderIdleBehaviour : IState<LeaderBehaviours>
    {
        private StateMachine<LeaderBehaviours> _stateMachine;
        private Leader _leader;

        public LeaderIdleBehaviour(StateMachine<LeaderBehaviours> stateMachine, Leader leader)
        {
            _stateMachine = stateMachine;
            _leader = leader;
        }
        
        public void OnEnter()
        {
            _leader.SetColor(Color.white);
            _leader.team.OnLaderChangedState?.Invoke(LeaderBehaviours.Idle);
        }

        public void OnExit()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            if (_leader.GetHealth() <= _leader.settings.FleeHealth) {
                _stateMachine.ChangeState(LeaderBehaviours.Flee);
                return;
            }

            if (_leader.AreEnemiesInLOS()) {
                _stateMachine.ChangeState(LeaderBehaviours.Attack);
            }

            if (Input.GetMouseButtonDown(_leader.settings.MouseClickCommand))
            {
                _leader._goal.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _stateMachine.ChangeState(LeaderBehaviours.Move);
            }
            
        }
    }
}