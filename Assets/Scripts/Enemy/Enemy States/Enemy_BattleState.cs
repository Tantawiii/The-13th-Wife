using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private float lastTimeInRange;

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
            enemy.SetVelocity(0, rb.linearVelocity.y);

            if (!Enemy.AttacksDisabled && Time.time >= enemy.LastAttackTime + enemy.attackCooldown)
                stateMachine.ChangeState(enemy.attackState);

            return;
        }

        bool settled = Time.time > stateEnterTime + JumpSettleTime && Time.time > lastFlipTime + JumpSettleTime;

        if (settled && enemy.groundDetected && (enemy.ledgeAhead || enemy.wallDetected))
        {
            if (enemy.HasJumpLanding(direction))
            {
                stateMachine.ChangeState(enemy.jumpState);
                return;
            }

            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        enemy.SetVelocity(enemy.battleMoveSpeed * direction, rb.linearVelocity.y);
    }
}
