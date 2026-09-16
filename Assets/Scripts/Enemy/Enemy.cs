using UnityEngine;

public class Enemy : Entity, IDamagable
{
    public Transform player { get; private set; }

    [Header("Patrol")]
    public float moveSpeed = 1.4f;
    public float idleTime = 2f;

    [Header("Battle")]
    public float battleMoveSpeed = 3f;
    public float attackRange = 1f;
    public float detectionRange = 6f;
    public float battleTimeDuration = 5f;

    [Header("Jump To Reach Player")]
    public float jumpForce = 12f;
    public float jumpTriggerHeight = 1.2f;
    public float jumpHorizontalRange = 4f;

    [Header("Health")]
    public int maxHits = 3;
    private int hitsTaken;

    public Enemy_IdleState idleState { get; private set; }
    public Enemy_MoveState moveState { get; private set; }
    public Enemy_BattleState battleState { get; private set; }
    public Enemy_AttackState attackState { get; private set; }
    public Enemy_JumpState jumpState { get; private set; }
    public Enemy_DeadState deadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        jumpState = new Enemy_JumpState(this, stateMachine, "battle");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public float GetHorizontalDistanceToPlayer() => player == null ? float.MaxValue : Mathf.Abs(player.position.x - transform.position.x);

    // Positive = player is above the enemy.
    public float GetVerticalDistanceToPlayer() => player == null ? 0f : player.position.y - transform.position.y;

    public bool PlayerInRange() => player != null && Vector2.Distance(transform.position, player.position) <= detectionRange;

    public void EnterBattleState()
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState || stateMachine.currentState == deadState)
            return;

        stateMachine.ChangeState(battleState);
    }

    public void TakeHit(Transform attacker)
    {
        if (stateMachine.currentState == deadState)
            return;

        hitsTaken++;
        EnterBattleState();

        if (hitsTaken >= maxHits)
            stateMachine.ChangeState(deadState);
    }
}
