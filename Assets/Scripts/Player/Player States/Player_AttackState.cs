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

        if (triggerCalled || stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
