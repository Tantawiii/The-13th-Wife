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
    }

    public override void Update()
    {
        base.Update();

        // triggerCalled is set by an Animation Event on the attack clip
        // (Entity_AnimationTriggers.CurrentStateTrigger) once one exists.
        // stateTimer is the fallback so this state can't get stuck forever
        // while there's no clip/event to fire it.
        if (triggerCalled || stateTimer < 0)
            stateMachine.ChangeState(enemy.battleState);
    }
}
