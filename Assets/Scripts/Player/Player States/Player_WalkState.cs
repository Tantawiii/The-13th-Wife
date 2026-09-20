using UnityEngine;

public class Player_WalkState : Player_GroundedState
{
    public Player_WalkState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        float moveInput = player.moveAction.ReadValue<Vector2>().x;

        if (Mathf.Abs(moveInput) < 0.01f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        player.SetVelocity(moveInput * player.walkSpeed, rb.linearVelocity.y);
    }
}
