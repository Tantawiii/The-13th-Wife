public class Player_AttackState : PlayerState
{
    public Player_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        // triggerCalled is set by an Animation Event on the Attack clip
        // (Entity_AnimationTriggers.CurrentStateTrigger) once one exists.
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
