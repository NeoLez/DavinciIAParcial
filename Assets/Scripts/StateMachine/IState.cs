using System;

namespace StateMachine {
    public interface IState <T> where T: Enum {
        public void OnEnter();
        public void OnExit();
        public void OnUpdate(float deltaTime);
    }
}