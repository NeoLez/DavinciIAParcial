using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Final.Scripts.EntityBehaviours {
    public class NPCSurrenderBehaviour : IState<NPCBehaviours>{
        private NPC _npc;
        private StateMachine<NPCBehaviours> _stateMachine;

        public NPCSurrenderBehaviour(StateMachine<NPCBehaviours> stateMachine, NPC npc)
        {
            _stateMachine = stateMachine;
            _npc = npc;
        }

        public void OnEnter() {
            _npc.SetColor(Color.gray);
        }

        public void OnExit() {
            
        }
        
        
        public void OnUpdate(float deltaTime) {

        }
    }
}