public class Player_AttackState : PlayerState
{
    public Player_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, rb.linearVelocity.y);
        stateTimer = player.attackDuration;
    }

    public override void Update()
    {
        base.Update();

        // triggerCalled is set by an Animation Event on the Attack clip
        // (Entity_AnimationTriggers.CurrentStateTrigger) once one exists.
        // stateTimer is the fallback so this state can't get stuck forever
        // while there's no clip/event to fire it.
        if (triggerCalled || stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
