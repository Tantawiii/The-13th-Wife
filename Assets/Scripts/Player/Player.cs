using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity, ISaveable, IDamagable
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

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    // Fallback so the state can always self-exit even before an Attack
    // animation/Animation Event exists (triggerCalled is still preferred once it does).
    public float attackDuration = 0.4f;

    [Header("Health")]
    public int maxHits = 3;
    public const int MaxPossibleHits = 10;
    private int hitsTaken;
    public int HitsTaken => hitsTaken;

    // Fired whenever hitsTaken or maxHits changes - lets UI_HealthDisplay
    // refresh reactively instead of polling every frame.
    public event System.Action OnHealthChanged;

    [SerializeField] private UI_LevelLose levelLoseUI;
    [SerializeField] private UI_FloatingPopup bonusLifePopup;

    public bool IsDead => stateMachine.currentState == deadState;

    public float defaultGravityScale { get; private set; }

    public Player_IdleState idleState { get; private set; }
    public Player_RunState runState { get; private set; }
    public Player_WalkState walkState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_AttackState attackState { get; private set; }
    public Player_DeadState deadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        defaultGravityScale = rb.gravityScale;

        // Restore any rebind overrides saved from a previous session before
        // anything below reads the (possibly now-different) bindings.
        RebindSaveLoad.Load(inputActions);

        InputActionMap playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");
        attackAction = playerMap.FindAction("Attack");

        idleState = new Player_IdleState(this, stateMachine, "idle");
        runState = new Player_RunState(this, stateMachine, "run");
        walkState = new Player_WalkState(this, stateMachine, "walk");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        attackState = new Player_AttackState(this, stateMachine, "attack");
        deadState = new Player_DeadState(this, stateMachine, "death");
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

    public override void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);

        foreach (Collider2D hit in hits)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>();
            damagable?.TakeHit(transform);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (attackPoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void TakeHit(Transform attacker)
    {
        if (stateMachine.currentState == deadState)
            return;

        hitsTaken++;
        ShakeCamera();
        OnHealthChanged?.Invoke();

        if (hitsTaken >= maxHits)
            Die();
    }

    // Called by an Enemy's death roll - a chance-based reward for fighting
    // through, capped so the fight never becomes trivial.
    public void GrantBonusLife()
    {
        if (maxHits >= MaxPossibleHits)
            return;

        maxHits++;
        OnHealthChanged?.Invoke();

        if (bonusLifePopup != null)
            bonusLifePopup.Show("+1 Life");
    }

    public void Die()
    {
        stateMachine.ChangeState(deadState);
        ScoreManager.Instance?.StopTracking();

        if (levelLoseUI != null)
            levelLoseUI.ShowLoseScreen();
    }

    public void LoadData(GameData data)
    {
        if (data.hasCheckpoint)
            transform.position = data.lastCheckpointPosition;
    }

    public void SaveData(ref GameData data)
    {
        // Checkpoint.SaveData already owns writing lastCheckpointPosition.
    }
}
