using UnityEngine;

namespace Hollow_Knight_Like.Scripts
{
    public abstract class EntityState
    {
        protected Player Player;
        protected StateMachine StateMachine;
        protected string StateName;

        protected Animator Anim;
        protected Rigidbody2D Rb;

        protected float StateTimer;
        protected bool TriggerCalled = false;

        public EntityState(Player player, StateMachine stateMachine, string stateName)
        {
            Player = player;
            StateMachine = stateMachine;
            StateName = stateName;
            Anim = player.anim;
            Rb = player.rb;
        }

        public virtual void Enter()
        {
            Anim.SetBool(StateName, true);
            TriggerCalled = false;
        }

        public virtual void Update()
        {
            Anim.SetFloat("yVelocity", Rb.linearVelocity.y);

            if (Player._input.Player.Dash.WasPressedThisFrame() && CanDash())
                StateMachine.ChangeState(Player.DashState);

            StateTimer -= Time.deltaTime;
        }

        public virtual void Exit()
        {
            Anim.SetBool(StateName, false);
        }

        public void CallAnimationTrigger()
        {
            TriggerCalled = true;
        }

        private bool CanDash()
        {
            if (Player.wallDetected)
                return false;

            if (StateMachine.CurrentState == Player.DashState)
                return false;

            return true;
        }
    }
}