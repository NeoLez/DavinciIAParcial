using System;

namespace StateMachine {
    public interface IState <T> where T: Enum {
        /// <summary>
        /// Executed when this state becomes the current one. Runs after the OnExit() method of the previous state.
        /// </summary>
        public void OnEnter();
        /// <summary>
        /// Executed when this state stops being the current state. Runs before the OnStart() method of the previous state.
        /// </summary>
        public void OnExit();
        public void OnUpdate(float deltaTime);
    }
}