public class Enemy_AttackState : EnemyState
{
    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        // triggerCalled is set by an Animation Event on the attack clip
        // (Entity_AnimationTriggers.CurrentStateTrigger) once one exists.
        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
