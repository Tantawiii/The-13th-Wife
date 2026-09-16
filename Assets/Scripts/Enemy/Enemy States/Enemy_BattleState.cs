using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private float lastTimeInRange;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        lastTimeInRange = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.player == null)
            return;

        if (enemy.PlayerInRange())
            lastTimeInRange = Time.time;

        if (Time.time > lastTimeInRange + enemy.battleTimeDuration)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        int direction = enemy.player.position.x > enemy.transform.position.x ? 1 : -1;
        enemy.HandleFlip(direction);

        float horizontalDistance = enemy.GetHorizontalDistanceToPlayer();
        float verticalDistance = enemy.GetVerticalDistanceToPlayer();

        if (horizontalDistance <= enemy.attackRange && Mathf.Abs(verticalDistance) <= enemy.jumpTriggerHeight)
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        // Player is meaningfully above and close enough horizontally to close the
        // gap with a jump - reuses the same running animation, since there's no
        // dedicated jump animation for enemies yet (game-jam scope).
        if (verticalDistance > enemy.jumpTriggerHeight && horizontalDistance <= enemy.jumpHorizontalRange && enemy.groundDetected)
        {
            stateMachine.ChangeState(enemy.jumpState);
            return;
        }

        enemy.SetVelocity(enemy.battleMoveSpeed * direction, rb.linearVelocity.y);
    }
}
