using System;
using System.Collections.Generic;

namespace StateMachine {
    public class StateMachine <T> where T: Enum {
        private Dictionary<T, IState<T>> _states = new();
        private IState<T> _currentState;

        public void ChangeState(T state) {
            if(_currentState!=null) _currentState.OnExit();
            if (_states.ContainsKey(state)) {
                _currentState = _states[state];
                _currentState.OnEnter();
            }
        }

        public void AddState(T type, IState<T> state) {
            if (_states.ContainsKey(type)) return;
            _states[type] = state;
        }

        public void UpdateState(float deltaTime) {
            _currentState.OnUpdate(deltaTime);
        }
    }
}