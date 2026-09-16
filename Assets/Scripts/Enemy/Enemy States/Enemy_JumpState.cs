using UnityEngine;

// No RPG2D equivalent - RPG2D enemies are strictly ground-locked. This lets an
// enemy close a vertical gap to reach the player on a platform above. It reuses
// battleState's own animBoolName, so the run animation keeps playing straight
// through the jump instead of needing a dedicated jump animation (game-jam scope).
public class Enemy_JumpState : EnemyState
{
    public Enemy_JumpState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, enemy.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.player != null)
        {
            int direction = enemy.player.position.x > enemy.transform.position.x ? 1 : -1;
            enemy.SetVelocity(enemy.battleMoveSpeed * direction, rb.linearVelocity.y);
        }

        if (enemy.groundDetected && rb.linearVelocity.y <= 0)
            stateMachine.ChangeState(enemy.battleState);
    }
}
