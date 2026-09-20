public class Enemy_AttackState : EnemyState
{
    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(0, rb.linearVelocity.y);
        stateTimer = enemy.attackDuration;
        enemy.PerformAttack();
        enemy.MarkAttackPerformed();
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled || stateTimer < 0)
            stateMachine.ChangeState(enemy.IsPlayerDead() ? enemy.idleState : enemy.battleState);
    }
}
