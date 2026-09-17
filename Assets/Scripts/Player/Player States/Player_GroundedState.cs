public abstract class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (!player.HasCoyoteGrounding())
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        if (player.jumpAction.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        if (player.attackAction.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.attackState);
        }
    }
}
