using UnityEngine;

namespace Hollow_Knight_Like.Scripts
{
    public class Player_MoveState : Player_GroundedState
    {
        public Player_MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine,
            stateName)
        {
        }

        public override void Update()
        {
            base.Update();

            if (Player.moveInput.x == 0)
                StateMachine.ChangeState(Player.IdleState);

            Player.SetVelocity(Player.moveInput.x * Player.moveSpeed, Rb.linearVelocity.y);
        }
    }
}