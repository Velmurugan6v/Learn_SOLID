namespace Hollow_Knight_Like.Scripts
{
    public class Player_DashState : EntityState
    {
        private float _originalGravity;

        public Player_DashState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine,
            stateName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _originalGravity = Rb.gravityScale;
            StateTimer = Player.dashDuration;
        }

        public override void Update()
        {
            base.Update();

            CancelDashIfNeeded();
            Player.SetVelocity(Player.dashSpeed * Player.FaceDirection, 0f);

            if (StateTimer < 0)
            {
                if (Player.groundDetected)
                    StateMachine.ChangeState(Player.IdleState);
                else
                    StateMachine.ChangeState(Player.FallState);
            }
        }

        public override void Exit()
        {
            base.Exit();

            Player.SetVelocity(0, 0);
            Rb.gravityScale = _originalGravity;
        }

        private void CancelDashIfNeeded()
        {
            if (Player.wallDetected)
            {
                if (Player.groundDetected)
                    StateMachine.ChangeState(Player.IdleState);
                else
                    StateMachine.ChangeState(Player.WallSlideState);
            }
        }
    }
}