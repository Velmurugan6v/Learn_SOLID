namespace Hollow_Knight_Like.Scripts
{
    public class Player_JumpState : Player_AiredState
    {
        public Player_JumpState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine,
            stateName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Player.SetVelocity(Rb.linearVelocity.x, Player.jumpForce);
        }

        public override void Update()
        {
            base.Update();

            if (Rb.linearVelocity.y < 0)
                StateMachine.ChangeState(Player.FallState);
        }
    }
}