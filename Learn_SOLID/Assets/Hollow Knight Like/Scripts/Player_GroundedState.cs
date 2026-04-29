namespace Hollow_Knight_Like.Scripts
{
    public class Player_GroundedState : EntityState
    {
        public Player_GroundedState(Player player, StateMachine stateMachine, string stateName) : base(player,
            stateMachine, stateName)
        {
        }


        public override void Update()
        {
            base.Update();

            if (Rb.linearVelocity.y < 0)
                StateMachine.ChangeState(Player.FallState);

            if (Player._input.Player.Jump.WasPressedThisFrame())
                StateMachine.ChangeState(Player.JumpState);

            if (Player._input.Player.Attack.WasPressedThisFrame())
                StateMachine.ChangeState(Player.BasicAttackState);
        }
    }
}