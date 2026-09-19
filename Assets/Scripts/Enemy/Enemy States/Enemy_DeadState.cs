using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        // No death animation exists yet, so this sells the death for free: pop
        // the enemy up and let it fall away with an exaggerated gravity scale.
        animator.enabled = false;

        rb.gravityScale = 12f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);

        enemy.GetComponent<Collider2D>().enabled = false;

        stateMachine.SwitchOffStateMachine();

        // Long enough to read the pop-and-fall before it's recycled back into
        // the spawner's pool.
        enemy.DelayedReturnToPool(2f);
    }
}
