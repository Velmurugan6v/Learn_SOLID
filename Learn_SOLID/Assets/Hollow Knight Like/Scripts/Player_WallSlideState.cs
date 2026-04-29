namespace Hollow_Knight_Like.Scripts
{
    public class Player_WallSlideState : EntityState
    {
        private float _originalGravity;

        public Player_WallSlideState(Player player, StateMachine stateMachine, string stateName) : base(player,
            stateMachine, stateName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _originalGravity = Rb.gravityScale;
        }

        public override void Update()
        {
            base.Update();

            HandleWallSlide();

            if (Player._input.Player.Jump.WasPressedThisFrame())
                StateMachine.ChangeState(Player.WallJumpState);

            if (!Player.wallDetected /*|| player.moveInput.x == 0*/)
            {
                StateMachine.ChangeState(Player.FallState);
                Player.Flip();
            }


            if (Player.groundDetected)
            {
                StateMachine.ChangeState(Player.IdleState);
                Player.Flip();
            }
        }

        private void HandleWallSlide()
        {
            if (Player.moveInput.y < 0)
                Player.SetVelocity(Player.moveInput.x, Rb.linearVelocity.y);
            else
                Player.SetVelocity(Player.moveInput.x, Rb.linearVelocity.y * Player.wallslidSlowMultiplier);
        }
    }
}