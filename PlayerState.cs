using Godot;
using Metal.AI.FSM;

namespace NX
{
    public partial class Player
    {
        class BaseState : State<Player>
        {
            protected Vector2 GetDir()
            {
                var dir = new Vector2();

                if (Input.IsActionPressed("ui_right"))
                {
                    dir.x += 1;
                }

                if (Input.IsActionPressed("ui_left"))
                {
                    dir.x -= 1;
                }

                if (Input.IsActionPressed("ui_down"))
                {
                    dir.y += 1;
                }

                if (Input.IsActionPressed("ui_up"))
                {
                    dir.y -= 1;
                }

                return dir;
            }
        }

        class Stand : BaseState
        {
            public override void OnEnter()
            {
                base.OnEnter();
                Owner._animationPlayer.Play("Jump");
            }

            public override void _Process(float delta)
            {
                base._Process(delta);
                var dir = GetDir();
                if (dir.x != 0)
                {
                    var scale = Owner.Scale;
                    scale.x = Mathf.Abs(scale.x) * dir.x;
                    Owner.Scale = scale;
                    Owner._machine.SetState(typeof(Walk).Name);
                }
                else if (dir.x < 0)
                {
                    Owner._animationPlayer.Play("StandUp");
                }
            }
        }

        class Walk : BaseState
        {
            public override void OnEnter()
            {
                base.OnEnter();
                var dir = GetDir();
                Owner._animationPlayer.Play("Walk");
            }
        }

        class Jump : BaseState
        {
        }

        class Fall : BaseState
        {
        }
    }
}