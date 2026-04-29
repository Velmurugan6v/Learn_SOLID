namespace Hollow_Knight_Like.Scripts
{
    public class Player_WallJumpState : EntityState
    {
        public Player_WallJumpState(Player player, StateMachine stateMachine, string stateName) : base(player,
            stateMachine, stateName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Player.SetVelocity(Player.wallJumpForce.x * -Player.FaceDirection, Player.wallJumpForce.y);
        }

        public override void Update()
        {
            base.Update();

            if (Rb.linearVelocity.y < 0)
                StateMachine.ChangeState(Player.FallState);

            if (Player.wallDetected)
                StateMachine.ChangeState(Player.WallSlideState);
        }
    }
}