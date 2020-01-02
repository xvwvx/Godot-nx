using Godot;
using Metal.AI.FSM;

namespace NX
{
    enum Direction : uint
    {
        None = 0x0000,
        Right = 0x0001,
        Left = 0x0002,
        Up = 0x0004,
        Down = 0x0008
    }

    public partial class Player
    {
        private class BaseState : State<Player>
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

        private class Stand : BaseState
        {
            public override void OnEnter()
            {
                base.OnEnter();
                Owner._animationPlayer.Play("Stand");
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
                    return;
                }

                if (dir.y == 0)
                {
                    Owner._animationPlayer.Play("Stand");
                }
                else if (dir.y < 0)
                {
                    Owner._animationPlayer.Play("StandUp");
                }
                else
                {
                    Owner._animationPlayer.Play("StandDown");
                }
            }
        }

        private class Walk : BaseState
        {
            public override void OnEnter()
            {
                base.OnEnter();
                var dir = GetDir();
                Owner._animationPlayer.Play("Walk");
            }

            public override void _Process(float delta)
            {
                base._Process(delta);

                var dir = GetDir();
                if (dir.x == 0)
                {
                    Owner._machine.SetState(typeof(Stand).Name);
                }
            }
        }

        private class Jump : BaseState
        {
            public override void _Process(float delta)
            {
                base._Process(delta);

                var dir = GetDir();
            }
        }

        private class Fall : BaseState
        {
            public override void _Process(float delta)
            {
                base._Process(delta);

                var dir = GetDir();
            }
        }
    }
}