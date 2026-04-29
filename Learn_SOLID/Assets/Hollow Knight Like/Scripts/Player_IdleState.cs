using UnityEngine;

namespace Hollow_Knight_Like.Scripts
{
    public class Player_IdleState : Player_GroundedState
    {
        public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine,
            stateName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Player.SetVelocity(0, Rb.linearVelocity.y);
        }

        public override void Update()
        {
            base.Update();

            if (Player.moveInput.x != 0)
                StateMachine.ChangeState(Player.MoveState);
        }
    }
}