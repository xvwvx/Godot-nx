using Godot;

namespace Metal.AI.FSM
{
    public abstract class State<T> : Node
        where T : Node
    {
        protected StateMachine<T> Machine => GetParent<StateMachine<T>>();
        protected T Owner => Machine.GetParent<T>();

        public override void _Ready()
        {
            base._Ready();
            OnInit();
        }

        public virtual void OnInit()
        {
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}