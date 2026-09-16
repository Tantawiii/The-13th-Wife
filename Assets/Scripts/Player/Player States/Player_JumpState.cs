using UnityEngine;

// Covers both the jump and the fall - Jump only has the one animation, so
// there's no separate Fall state. yVelocity (synced every frame by
// PlayerState) is what a blend tree would read to pick a rising/falling pose
// out of that single Animator state.
public class Player_JumpState : PlayerState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Only apply the jump impulse if we entered by pressing Jump while
        // grounded. If we entered because the ground was lost mid-run, we're
        // already falling and should keep the current vertical velocity.
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
