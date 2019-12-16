using Godot;
using Metal.AI.FSM;
using NX.Arms.Weapon;

namespace NX
{
    public partial class Player : Node2D
    {
        public readonly StateMachine<Player> _machine = new StateMachine<Player>(
            new Stand(), new Walk(), new Jump(), new Fall()
        );

        private AnimationPlayer _animationPlayer;
        private Weapon _weapon;

        public override void _Ready()
        {
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _weapon = GetNode<Weapon>("Weapon/Body");

            AddChild(_machine);
            _machine.SetState(typeof(Stand).Name);
        }
    }
}