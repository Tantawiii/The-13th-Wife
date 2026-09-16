using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    public InputAction moveAction { get; private set; }
    public InputAction jumpAction { get; private set; }
    public InputAction attackAction { get; private set; }

    [Header("Move Info")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Ledge Climb")]
    [SerializeField] private LedgeDetection ledgeDetection;
    public Vector2 ledgeClimbOffset1;
    public Vector2 ledgeClimbOffset2;

    public float defaultGravityScale { get; private set; }

    public Player_IdleState idleState { get; private set; }
    public Player_RunState runState { get; private set; }
    public Player_WalkState walkState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_AttackState attackState { get; private set; }
    public Player_LedgeClimbState ledgeClimbState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        defaultGravityScale = rb.gravityScale;

        InputActionMap playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");
        attackAction = playerMap.FindAction("Attack");

        idleState = new Player_IdleState(this, stateMachine, "idle");
        runState = new Player_RunState(this, stateMachine, "run");
        walkState = new Player_WalkState(this, stateMachine, "walk");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        attackState = new Player_AttackState(this, stateMachine, "attack");
        ledgeClimbState = new Player_LedgeClimbState(this, stateMachine, "ledgeClimb");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    public LedgeDetection GetLedgeDetection() => ledgeDetection;
}
