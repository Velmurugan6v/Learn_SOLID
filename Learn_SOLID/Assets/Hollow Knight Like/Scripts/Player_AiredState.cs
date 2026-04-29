namespace Hollow_Knight_Like.Scripts
{
    public class Player_AiredState : EntityState
    {
        public Player_AiredState(Player player, StateMachine stateMachine, string stateName) : base(player,
            stateMachine, stateName)
        {
        }

        public override void Update()
        {
            base.Update();

            if (Player.wallDetected && Player.moveInput.x != 0)
                StateMachine.ChangeState(Player.WallSlideState);

            if (Player.moveInput.x != 0)
                Player.SetVelocity(Player.moveInput.x * (Player.moveSpeed * Player.midAirMultiplier),
                    Rb.linearVelocity.y);
        }
    }
}