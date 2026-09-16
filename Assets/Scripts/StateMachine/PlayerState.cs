using UnityEngine;

public abstract class PlayerState : EntityState
{
    protected Player player;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;
        animator = player.animator;
        rb = player.rb;
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }
}
