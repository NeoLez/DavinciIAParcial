using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Final.Scripts.EntityBehaviours {
    public class NPCIdleBehaviour : IState<NPCBehaviours>{
        private NPC _npc;
        private StateMachine<NPCBehaviours> _stateMachine;

        public NPCIdleBehaviour(StateMachine<NPCBehaviours> stateMachine, NPC npc)
        {
            _stateMachine = stateMachine;
            _npc = npc;
        }

        public void OnEnter() {
            _npc.team.OnLaderChangedState += SwitchToMove;
            _npc.SetColor(Color.white);
        }

        public void OnExit() {
            _npc.team.OnLaderChangedState -= SwitchToMove;
        }

        private void SwitchToMove(LeaderBehaviours behaviour) {
            if (behaviour == LeaderBehaviours.Move) {
                Debug.Log("a");
                _stateMachine.ChangeState(NPCBehaviours.Move);
            }
        }
        
        public void OnUpdate(float deltaTime) {
            if (_npc.GetHealth() <= _npc.settings.FleeHealth) {
                _stateMachine.ChangeState(NPCBehaviours.Flee);
                return;
            }

            if (_npc.AreEnemiesInLOS()) {
                _stateMachine.ChangeState(NPCBehaviours.Attack);
            }

            if (Vector2.Distance(_npc.transform.position, _npc.team.leader.transform.position) > _npc.settings.LeaderCloseAreaRadius || !PointManager.Instance.ArePointsInView(_npc.team.leader._point, _npc._point)) {
                Debug.Log("b");
                _stateMachine.ChangeState(NPCBehaviours.Move);
            }
        }
    }
}