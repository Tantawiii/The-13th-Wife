using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private float lastTimeInRange;

    // Jumping is legitimate mid-chase (the player jumped to another platform
    // across a gap) - but right on Enter(), or right after HandleFlip() spins
    // the enemy around, a stale/transient vertical-distance reading can
    // spuriously clear jumpTriggerHeight even on flat shared ground. This
    // short settle window after either event filters that out without
    // blocking the real cross-platform jump.
    private const float JumpSettleTime = 0.15f;
    private float stateEnterTime;
    private float lastFlipTime;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        lastTimeInRange = Time.time;
        stateEnterTime = Time.time;
        lastFlipTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.player == null || enemy.IsPlayerDead())
        {
            enemy.SetVelocity(0, rb.linearVelocity.y);
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (enemy.PlayerInRange())
            lastTimeInRange = Time.time;

        if (Time.time > lastTimeInRange + enemy.battleTimeDuration)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        int previousFacingDir = enemy.facingDir;
        int direction = enemy.player.position.x > enemy.transform.position.x ? 1 : -1;
        enemy.HandleFlip(direction);

        if (enemy.facingDir != previousFacingDir)
            lastFlipTime = Time.time;

        float horizontalDistance = enemy.GetHorizontalDistanceToPlayer();
        float verticalDistance = enemy.GetVerticalDistanceToPlayer();

        if (horizontalDistance <= enemy.attackRange && Mathf.Abs(verticalDistance) <= enemy.jumpTriggerHeight)
        {
            // Hold position at range even while on cooldown, rather than
            // re-attacking the instant the previous attack ends.
            enemy.SetVelocity(0, rb.linearVelocity.y);

            if (Time.time >= enemy.LastAttackTime + enemy.attackCooldown)
                stateMachine.ChangeState(enemy.attackState);

            return;
        }

        // Player is meaningfully above and close enough horizontally to close the
        // gap with a jump - reuses the same running animation, since there's no
        // dedicated jump animation for enemies yet (game-jam scope). Gated by
        // JumpSettleTime so it only fires once things have settled since
        // entering battle or last flipping (see field comment above).
        bool settled = Time.time > stateEnterTime + JumpSettleTime && Time.time > lastFlipTime + JumpSettleTime;

        if (settled && verticalDistance > enemy.jumpTriggerHeight && horizontalDistance <= enemy.jumpHorizontalRange && enemy.groundDetected)
        {
            stateMachine.ChangeState(enemy.jumpState);
            return;
        }

        enemy.SetVelocity(enemy.battleMoveSpeed * direction, rb.linearVelocity.y);
    }
}
