using UnityEngine;

// Not entered from player input - Run is the default grounded movement.
// This state is only reachable via an external call to
// stateMachine.ChangeState(player.walkState), e.g. from a Timeline signal
// or cutscene script that needs slower, scripted movement.
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
