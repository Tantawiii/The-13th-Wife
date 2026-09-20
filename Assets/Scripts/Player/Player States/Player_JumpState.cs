using UnityEngine;

public class Player_JumpState : PlayerState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (player.groundDetected)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        float moveInput = player.moveAction.ReadValue<Vector2>().x;
        player.SetVelocity(moveInput * player.runSpeed, rb.linearVelocity.y);

        if (player.groundDetected && rb.linearVelocity.y <= 0)
            stateMachine.ChangeState(player.idleState);
    }
}
