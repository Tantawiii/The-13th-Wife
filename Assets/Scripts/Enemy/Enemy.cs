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

    public static bool AttacksDisabled { get; set; }

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
    public float attackDuration = 0.4f;
    [Tooltip("Minimum time between attacks - without this, staying in range re-triggers the attack animation the instant it ends, reading as spammy.")]
    public float attackCooldown = 1f;
    public float LastAttackTime { get; private set; } = -999f;
    public void MarkAttackPerformed() => LastAttackTime = Time.time;

    [Header("Jump To Reach Player")]
    public float jumpForce = 12f;
    public float jumpTriggerHeight = 1.2f;

    [Header("Jump Landing Check")]
    [Tooltip("Radius checked for ground at each sampled point along the simulated jump arc.")]
    [SerializeField] private float landingCheckRadius = 0.3f;
    [Tooltip("How many points along the simulated jump arc (and the fall that follows it) to test for a safe landing.")]
    [SerializeField] private int landingCheckSamples = 12;
    [Tooltip("How long to simulate the jump + fall for, in seconds - generous enough to cover landing well below the takeoff point too, not just back at launch height.")]
    [SerializeField] private float landingCheckMaxTime = 2f;

    public bool HasJumpLanding(int direction)
    {
        float gravity = Physics2D.gravity.y * rb.gravityScale;

        if (gravity >= 0f)
            return false;

        Vector2 previous = transform.position;

        for (int i = 1; i <= landingCheckSamples; i++)
        {
            float t = landingCheckMaxTime * i / landingCheckSamples;
            float x = battleMoveSpeed * direction * t;
            float y = jumpForce * t + 0.5f * gravity * t * t;

            Vector2 point = (Vector2)transform.position + new Vector2(x, y);
            Vector2 segment = point - previous;
            float distance = segment.magnitude;

            RaycastHit2D hit = distance > 0.001f
                ? Physics2D.CircleCast(previous, landingCheckRadius, segment / distance, distance, GetWhatIsGround())
                : Physics2D.CircleCast(previous, landingCheckRadius, Vector2.down, 0f, GetWhatIsGround());

            if (hit.collider != null && hit.normal.y > 0.5f)
                return true;

            previous = point;
        }

        return false;
    }

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
        feetOffset = col != null ? transform.position.y - col.bounds.min.y : 0f;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerScript = playerObj.GetComponent<Player>();
        }

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
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

    public float GetFeetOffset() => feetOffset;

    public float GetHorizontalDistanceToPlayer() => player == null ? float.MaxValue : Mathf.Abs(player.position.x - transform.position.x);

    public float GetVerticalDistanceToPlayer() => player == null ? 0f : player.position.y - transform.position.y;

    public bool IsPlayerDead() => playerScript != null && playerScript.IsDead;

    public bool PlayerInRange() => player != null && !IsPlayerDead() && GetHorizontalDistanceToPlayer() <= detectionRange;

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

    public void KillForBossVictory()
    {
        if (stateMachine.currentState == deadState)
            return;

        stateMachine.ChangeState(deadState);
        ScoreManager.Instance?.RegisterEnemyDefeated();
    }

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
        if (AttacksDisabled || player == null || IsPlayerDead())
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

        DrawJumpArcGizmo(1);
        DrawJumpArcGizmo(-1);
    }

    private void DrawJumpArcGizmo(int direction)
    {
        if (rb == null)
            return;

        float gravity = Physics2D.gravity.y * rb.gravityScale;

        if (gravity >= 0f)
            return;

        Gizmos.color = Color.yellow;
        Vector3 previous = transform.position;

        for (int i = 1; i <= landingCheckSamples; i++)
        {
            float t = landingCheckMaxTime * i / landingCheckSamples;
            float x = battleMoveSpeed * direction * t;
            float y = jumpForce * t + 0.5f * gravity * t * t;

            Vector3 point = transform.position + new Vector3(x, y, 0f);
            Gizmos.DrawLine(previous, point);
            previous = point;
        }
    }

    public void PrepareForDraw()
    {
        SetSpriteAlpha(0f);

        if (col != null)
            col.enabled = false;

        rb.simulated = false;
    }

    public void SetDrawProgress(float t) => SetSpriteAlpha(t);

    private void SetSpriteAlpha(float alpha)
    {
        if (spriteRenderer == null)
            return;

        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

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
