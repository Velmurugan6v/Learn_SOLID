using System;
using System.Collections;
using UnityEngine;

namespace Hollow_Knight_Like.Scripts
{
    public class Player : MonoBehaviour
    {
        public Animator anim { get; private set; }
        public Rigidbody2D rb { get; private set; }

        private StateMachine _stateMachine;

        public PlayerInputSet _input { get; private set; }
        public Player_IdleState IdleState { get; private set; }
        public Player_MoveState MoveState { get; private set; }
        public Player_JumpState JumpState { get; private set; }
        public Player_FallState FallState { get; private set; }
        public Player_WallSlideState WallSlideState { get; private set; }
        public Player_WallJumpState WallJumpState { get; private set; }
        public Player_DashState DashState { get; private set; }
        public Player_BasicAttackState BasicAttackState { get; private set; }

        [Header("Attack details")] public Vector2[] attackVelocity;
        public float attackVeloictyDuration = 0.1f;
        public int comboResetTime = 1;
        private Coroutine quededAttackCo;


        [Header("Movement details")] public float moveSpeed;
        public float jumpForce = 8;
        private bool _isFacingRight = true;
        public Vector2 wallJumpForce;
        public Vector2 moveInput { get; private set; }


        [Header("Collision detection")] [SerializeField]
        private float groundCheckDistance;

        [Range(0, 1)] public float midAirMultiplier;
        [SerializeField] private LayerMask whatIsGround;
        public bool groundDetected { get; private set; }

        [Header("Wall detection")] [SerializeField]
        private float wallCheckDistance;

        [Range(0, 1)] public float wallslidSlowMultiplier;
        public bool wallDetected { get; private set; }
        public int FaceDirection { get; private set; } = 1;

        [Space(10)] public float dashDuration = 0.25f;
        public float dashSpeed = 20;

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();

            _stateMachine = new StateMachine();
            _input = new PlayerInputSet();

            IdleState = new Player_IdleState(this, _stateMachine, "Idle");
            MoveState = new Player_MoveState(this, _stateMachine, "Move");
            JumpState = new Player_JumpState(this, _stateMachine, "JumpFall");
            FallState = new Player_FallState(this, _stateMachine, "JumpFall");
            WallSlideState = new Player_WallSlideState(this, _stateMachine, "WallSlide");
            WallJumpState = new Player_WallJumpState(this, _stateMachine, "JumpFall");
            DashState = new Player_DashState(this, _stateMachine, "Dash");
            BasicAttackState = new Player_BasicAttackState(this, _stateMachine, "BasicAttack");
        }

        private void OnEnable()
        {
            _input.Enable();
            _input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            _input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void Start()
        {
            _stateMachine.InitializeState(IdleState);
        }

        private void Update()
        {
            HandleGroundCheckDetection();
            HandleWallCheckDetection();
            _stateMachine.CurrentState.Update();
        }

        public void EnterAttackStateWithDelay()
        {
            if (quededAttackCo != null)
                StopCoroutine(quededAttackCo);

            quededAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
        }

        private IEnumerator EnterAttackStateWithDelayCo()
        {
            yield return new WaitForEndOfFrame();
            _stateMachine.ChangeState(BasicAttackState);
        }

        public void SetVelocity(float xVelocity, float yVelocity)
        {
            rb.linearVelocity = new Vector2(xVelocity, yVelocity);
            HandleFlip(xVelocity);
        }

        public void CallAnimationTrigger()
        {
            _stateMachine.CurrentState.CallAnimationTrigger();
        }

        private void HandleFlip(float xVelocity)
        {
            if (xVelocity < 0 && _isFacingRight)
                Flip();
            else if (xVelocity > 0 && !_isFacingRight)
                Flip();
        }

        public void Flip()
        {
            transform.Rotate(0f, 180f, 0f);
            _isFacingRight = !_isFacingRight;
            ChangeWallSlideFaceDirection();
        }

        private void HandleGroundCheckDetection()
        {
            groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        }

        private void HandleWallCheckDetection()
        {
            wallDetected = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance * FaceDirection,
                whatIsGround);
        }

        private void ChangeWallSlideFaceDirection()
        {
            FaceDirection *= -1;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(wallCheckDistance * FaceDirection, 0));
        }
    }
}