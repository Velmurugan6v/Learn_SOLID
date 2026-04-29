namespace Hollow_Knight_Like.Scripts
{
    public class Player_FallState : Player_AiredState
    {
        public Player_FallState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
        {
        }

        public override void Update()
        {
            base.Update();
            
            if(Player.groundDetected)
                StateMachine.ChangeState(Player.IdleState);
        }
    }
}