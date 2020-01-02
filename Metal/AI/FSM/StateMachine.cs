using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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


        public bool HasState(string key)
        {
            return _states[key] != null;
        }

        public bool HasState<StateType>() where StateType : State<T>
        {
            return _states[typeof(StateType).Name] != null;
        }

        public State<T> AddState(string key)
        {
            var assemblyName = typeof(T).Assembly.FullName;
            var fullName = typeof(T).FullName;

            var state = (State<T>) Activator.CreateInstance(assemblyName, fullName + "." + key).Unwrap();
            _states[state.Name] = state;
            return state;
        }

        public void AddState(State<T> state)
        {
            _states[state.Name] = state;
        }

        public StateType AddState<StateType>() where StateType : State<T>
        {
            var state = Activator.CreateInstance<StateType>();
            _states[state.Name] = state;
            return state;
        }

        public bool RemoveState(string key)
        {
            return _states.Remove(key);
        }

        public bool RemoveState(State<T> state)
        {
            return _states.Remove(state.Name);
        }

        public bool RemoveState<StateType>() where StateType : State<T>
        {
            return _states.Remove(typeof(StateType).Name);
        }

        public StateType GetState<StateType>(string key) where StateType : State<T>
        {
            return (StateType) _states[key];
        }

        public StateType GetState<StateType>() where StateType : State<T>
        {
            return (StateType) _states[typeof(StateType).Name];
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