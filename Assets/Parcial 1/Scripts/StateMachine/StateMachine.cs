using System;
using System.Collections.Generic;

namespace StateMachine {
    public class StateMachine <T> where T: Enum {
        private Dictionary<T, IState<T>> _states = new();
        private IState<T> _currentState;

        /// <summary>
        /// Changes state to the one associated to the key. If there is no associated state, stops machine execution.
        /// </summary>
        /// <param name="state">Enum key of the state</param>
        public void ChangeState(T state) {
            if(_currentState!=null) _currentState.OnExit();
            if (_states.ContainsKey(state)) {
                _currentState = _states[state];
                _currentState.OnEnter();
            }
        }

        /// <summary>
        /// Adds a new state to the machine under the enum key.
        /// If there already is a state associated to that key, it is NOT overwritten.
        /// </summary>
        /// <param name="type">Enum key of the state</param>
        /// <param name="state">State to be added</param>
        public void AddState(T type, IState<T> state) {
            if (_states.ContainsKey(type)) return;
            _states[type] = state;
        }

        public void UpdateState(float deltaTime) {
            _currentState.OnUpdate(deltaTime);
        }
    }
}