using System.Collections;
using UnityEngine;

public class Enemy : Entity, IDamagable
{
    public Transform player { get; private set; }
    private Player playerScript;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private EnemyPool sourcePool;
    private float defaultGravityScale;
    private float feetOffset;
    private bool frozen;

    [Header("Patrol")]
    public float moveSpeed = 1.4f;
    public float idleTime = 2f;
    [Tooltip("Off for enemy types with no idle Animator state - they turn around at walls/ledges instead of stopping, since routing them through Enemy_IdleState just freezes their animation on a bool their controller doesn't have a state for.")]
    public bool canIdle = true;

    [Header("Boss")]
    [Tooltip("Shadya, hand-placed at the end - her death clears every other active enemy, stops all spawners, and scores them all too.")]
    public bool isBoss = false;
    [SerializeField] private UI_BossVictory bossVictoryUI;

    [Header("Battle")]
    public float battleMoveSpeed = 3f;
    public float attackRange = 1f;
    public float detectionRange = 6f;
    public float battleTimeDuration = 5f;
    // Fallback so the state can always self-exit even before an attack
    // animation/Animation Event exists (no dedicated attack clip yet - game-jam scope).
    public float attackDuration = 0.4f;
    [Tooltip("Minimum time between attacks - without this, staying in range re-triggers the attack animation the instant it ends, reading as spammy.")]
    public float attackCooldown = 1f;
    public float LastAttackTime { get; private set; } = -999f;
    public void MarkAttackPerformed() => LastAttackTime = Time.time;

    [Header("Jump To Reach Player")]
    public float jumpForce = 12f;
    public float jumpTriggerHeight = 1.2f;
    public float jumpHorizontalRange = 4f;

    [Header("Health")]
    public int maxHits = 3;
    private int hitsTaken;

    [Header("Bonus Life")]
    [Range(0f, 1f)]
    [Tooltip("Chance, when the player lands the killing hit, of granting Player a bonus life (see Player.GrantBonusLife - capped, and shows a '+1 Life' popup).")]
    public float bonusLifeChance = 0.15f;

    public Enemy_IdleState idleState { get; private set; }
    public Enemy_MoveState moveState { get; private set; }
    public Enemy_BattleState battleState { get; private set; }
    public Enemy_AttackState attackState { get; private set; }
    public Enemy_JumpState jumpState { get; private set; }
    public Enemy_DeadState deadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        defaultGravityScale = rb.gravityScale;
        // How far below this transform's own origin the collider's bottom
        // edge sits - position-independent, so it stays correct no matter
        // where the enemy is later moved (e.g. by the spawner).
        feetOffset = col != null ? transform.position.y - col.bounds.min.y : 0f;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerScript = playerObj.GetComponent<Player>();
        }

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        // Move/Battle/Jump all share the "run" Animator bool - Enemy_AC only
        // has idle/run/attack states, no dedicated move or battle animation.
        moveState = new Enemy_MoveState(this, stateMachine, "run");
        battleState = new Enemy_BattleState(this, stateMachine, "run");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        jumpState = new Enemy_JumpState(this, stateMachine, "run");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(canIdle ? idleState : moveState);
    }

    // While frozen (boss-gate timeline), stop AI/collision updates and physics
    // entirely - animation keeps whatever state it was already in.
    protected override void Update()
    {
        if (frozen)
            return;

        base.Update();
    }

    public void SetFrozen(bool value)
    {
        if (frozen == value)
            return;

        frozen = value;

        if (rb != null)
        {
            if (value)
                SetVelocity(0f, 0f);

            rb.simulated = !value;
        }
    }

    public float GetFeetOffset() => feetOffset;

    public float GetHorizontalDistanceToPlayer() => player == null ? float.MaxValue : Mathf.Abs(player.position.x - transform.position.x);

    // Positive = player is above the enemy.
    public float GetVerticalDistanceToPlayer() => player == null ? 0f : player.position.y - transform.position.y;

    public bool IsPlayerDead() => playerScript != null && playerScript.IsDead;

    public bool PlayerInRange() => player != null && !IsPlayerDead() && Vector2.Distance(transform.position, player.position) <= detectionRange;

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
        ShakeCamera(0.5f);
        EnterBattleState();

        if (hitsTaken >= maxHits)
            Die();
    }

    private void Die()
    {
        stateMachine.ChangeState(deadState);
        ScoreManager.Instance?.RegisterEnemyDefeated();

        if (Random.value < bonusLifeChance)
            playerScript?.GrantBonusLife();

        if (isBoss)
        {
            EnemySpawner.StopAllSpawning();
            ScoreManager.Instance?.StopTracking();

            foreach (Enemy other in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                if (other != this)
                    other.KillForBossVictory();
            }

            bossVictoryUI?.ShowVictory();
        }
    }

    // Called on every other active enemy the instant the boss dies - they all
    // go down and score with her, per the "beat Shadya = everyone else with
    // her dies too" design.
    public void KillForBossVictory()
    {
        if (stateMachine.currentState == deadState)
            return;

        stateMachine.ChangeState(deadState);
        ScoreManager.Instance?.RegisterEnemyDefeated();
    }

    // Called by a fall-death trigger - walked/knocked off the level entirely.
    // No death animation to play (it's already falling), so this skips
    // straight to scoring and pooling instead of going through deadState.
    public void FallOut()
    {
        if (stateMachine.currentState == deadState)
            return;

        stateMachine.SwitchOffStateMachine();
        ScoreManager.Instance?.RegisterEnemyDefeated();
        DelayedReturnToPool(0f);
    }

    public override void PerformAttack()
    {
        if (player == null || IsPlayerDead())
            return;

        if (GetHorizontalDistanceToPlayer() <= attackRange && Mathf.Abs(GetVerticalDistanceToPlayer()) <= jumpTriggerHeight)
        {
            IDamagable damagable = player.GetComponent<IDamagable>();
            damagable?.TakeHit(transform);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // Called by EnemySpawner right after pulling this instance out of the
    // pool - hides it (invisible, no physics, no collision) while the brush
    // draw-in plays, before FinishSpawn() actually turns it loose.
    public void PrepareForDraw()
    {
        SetSpriteAlpha(0f);

        if (col != null)
            col.enabled = false;

        rb.simulated = false;
    }

    // Called every frame of the brush stroke, t going 0 -> 1.
    public void SetDrawProgress(float t) => SetSpriteAlpha(t);

    private void SetSpriteAlpha(float alpha)
    {
        if (spriteRenderer == null)
            return;

        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    // Called once the brush stroke finishes - resets everything the previous
    // life (or Enemy_DeadState) may have left disabled/altered, and puts the
    // enemy back into a fresh idle state.
    public void FinishSpawn(EnemyPool pool)
    {
        sourcePool = pool;

        SetSpriteAlpha(1f);

        if (col != null)
            col.enabled = true;

        rb.simulated = true;
        rb.gravityScale = defaultGravityScale;
        animator.enabled = true;
        hitsTaken = 0;

        stateMachine.Initialize(canIdle ? idleState : moveState);
    }

    public void DelayedReturnToPool(float delay) => StartCoroutine(DelayedReturnToPoolCo(delay));

    private IEnumerator DelayedReturnToPoolCo(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (sourcePool != null)
            sourcePool.Release(this);
        else
            gameObject.SetActive(false);
    }
}
