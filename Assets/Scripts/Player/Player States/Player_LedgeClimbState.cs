using UnityEngine;

public class Player_LedgeClimbState : PlayerState
{
    private Vector2 climbBeginPosition;
    private Vector2 climbEndPosition;

    public Player_LedgeClimbState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        LedgeDetection ledgeDetection = player.GetLedgeDetection();
        ledgeDetection.SetCanGrabLedge(false);

        Vector2 ledgePosition = ledgeDetection.transform.position;
        climbBeginPosition = ledgePosition + player.ledgeClimbOffset1;
        climbEndPosition = ledgePosition + player.ledgeClimbOffset2;

        player.transform.position = climbBeginPosition;
    }

    public override void Update()
    {
        base.Update();

        // Ported from Endless-Runner's ledge climb: that repo's source
        // spritesheet bakes the climbing motion directly into its frames
        // (confirmed via its Player_LedgeClimb.anim - m_PositionCurves was
        // empty, only a sprite-swap PPtrCurve), so the transform is pinned
        // here for the whole clip and only snaps forward at the end. If this
        // project's own Ledge Climb spritesheet does NOT bake the movement
        // into its frames, replace this pin with a Lerp from
        // climbBeginPosition to climbEndPosition instead.
        player.transform.position = climbBeginPosition;

        if (triggerCalled)
            ExitLedgeClimb();
    }

    private void ExitLedgeClimb()
    {
        rb.gravityScale = player.defaultGravityScale;
        player.transform.position = climbEndPosition;
        player.GetLedgeDetection().ReenableAfterDelay(0.1f);
        stateMachine.ChangeState(player.idleState);
    }
}
