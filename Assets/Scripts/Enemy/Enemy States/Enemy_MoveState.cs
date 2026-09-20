public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (!enemy.groundDetected || enemy.wallDetected || enemy.ledgeAhead)
            enemy.Flip();
    }

    public override void Update()
    {
        base.Update();

        if (!enemy.groundDetected || enemy.wallDetected || enemy.ledgeAhead)
        {
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
