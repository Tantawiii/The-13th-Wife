using UnityEngine;

public class Player_RunState : Player_GroundedState
{
    public Player_RunState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
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

        player.SetVelocity(moveInput * player.runSpeed, rb.linearVelocity.y);
    }
}
