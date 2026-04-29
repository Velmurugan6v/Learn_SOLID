using UnityEngine;

namespace Hollow_Knight_Like.Scripts
{
    public class Player_BasicAttackState : EntityState
    {
        private const int FirstComboIndex = 1;
        private int comboIndex = 1;
        private int comboLimit = 3;
        private float lastTimeAttacked;
        private float attackVelocityTimer;
        private bool comboAttackqueded;
        private int facingDirection;

        public Player_BasicAttackState(Player player, StateMachine stateMachine, string stateName) : base(player,
            stateMachine, stateName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            comboAttackqueded = false;
            ResetAttackComboIfNeeded();

            facingDirection = (Player.moveInput.x != 0) ? (int)Player.moveInput.x : Player.FaceDirection;

            Anim.SetInteger("BasicAttackIndex", comboIndex);
            ApplyAttackVelocity();
        }


        public override void Update()
        {
            base.Update();
            HandleAttackVelocity();

            if (Player._input.Player.Attack.WasPressedThisFrame())
                QueNextAttack();

            if (TriggerCalled)
                HandleExitState();
        }


        public override void Exit()
        {
            base.Exit();
            comboIndex++;
            lastTimeAttacked = Time.time;
        }

        private void HandleExitState()
        {
            if (comboAttackqueded)
            {
                Anim.SetBool(StateName, false);
                Player.EnterAttackStateWithDelay();
            }
            else
                StateMachine.ChangeState(Player.IdleState);
        }

        private void QueNextAttack()
        {
            if (comboIndex < comboLimit)
                comboAttackqueded = true;
        }

        private void HandleAttackVelocity()
        {
            attackVelocityTimer -= Time.deltaTime;

            if (attackVelocityTimer < 0)
                Player.SetVelocity(0, Rb.linearVelocity.y);
        }

        private void ApplyAttackVelocity()
        {
            Vector2 attackVelocity = Player.attackVelocity[comboIndex - 1];

            attackVelocityTimer = Player.attackVeloictyDuration;
            Player.SetVelocity(attackVelocity.x * Player.FaceDirection, attackVelocity.y);
        }

        private void ResetAttackComboIfNeeded()
        {
            if (Time.time > lastTimeAttacked + Player.comboResetTime)
                comboIndex = FirstComboIndex;

            if (comboIndex > comboLimit)
                comboIndex = FirstComboIndex;
        }
    }
}