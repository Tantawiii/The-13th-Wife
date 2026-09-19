public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Already facing a ledge/wall from the last time this state ran - turn
        // around before setting off again.
        if (!enemy.groundDetected || enemy.wallDetected || enemy.ledgeAhead)
            enemy.Flip();
    }

    public override void Update()
    {
        base.Update();

        if (!enemy.groundDetected || enemy.wallDetected || enemy.ledgeAhead)
        {
            // Enemy types without an idle animation just turn around in place
            // and keep going - routing them through Enemy_IdleState leaves
            // their Animator on a bool it doesn't have a state for, freezing
            // the animation instead of looking idle.
            if (!enemy.canIdle)
            {
                enemy.Flip();
                enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocity.y);
                return;
            }

            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocity.y);
    }
}
