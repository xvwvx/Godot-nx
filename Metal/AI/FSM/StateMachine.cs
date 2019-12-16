using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Metal.AI.FSM
{
    public class StateMachine<T> : Node
        where T : Node
    {
        #region variable

        public event Action OnStateChanged;
        public State<T> CurrentState { get; private set; }

        private readonly Stack<State<T>> _stack = new Stack<State<T>>();
        private readonly Dictionary<string, State<T>> _states;

        #endregion

        public StateMachine(params State<T>[] states)
        {
            _states = new Dictionary<string, State<T>>(states.Length);
            foreach (var state in states)
            {
                _states[state.GetType().Name] = state;
            }
        }

        private void Activate(State<T> state)
        {
            AddChild(state);
            state.OnEnter();
        }

        private void Deactivate(State<T> state)
        {
            state.OnExit();
            RemoveChild(state);
        }

        public void SetState(string key)
        {
            if (CurrentState != null)
            {
                Deactivate(CurrentState);
            }

            CurrentState = _states[key];
            Activate(CurrentState);

            OnStateChanged?.Invoke();
        }

        public void PopState()
        {
            Deactivate(CurrentState);
            CurrentState = _stack.Pop();
            Activate(CurrentState);

            OnStateChanged?.Invoke();
        }

        public void PushState(string key)
        {
            Debug.Assert(CurrentState.GetType().Name != key, "当前状态与上一个状态不能相同");

            Deactivate(CurrentState);
            _stack.Push(CurrentState);
            CurrentState = _states[key];
            Activate(CurrentState);

            OnStateChanged?.Invoke();
        }
    }
}