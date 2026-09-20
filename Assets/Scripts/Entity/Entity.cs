using Unity.Cinemachine;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }

    protected StateMachine stateMachine;

    private CinemachineImpulseSource impulseSource;

    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;

    [Header("Collision Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [Tooltip("How far ahead of groundCheck (in facing direction) to look for an upcoming ledge.")]
    [SerializeField] private float ledgeCheckAheadDistance = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float coyoteTime = 0.1f;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    public bool ledgeAhead { get; private set; }

    private float lastGroundedTime = -999f;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    protected void ShakeCamera(float force = 1f) => impulseSource?.GenerateImpulseWithForce(force);

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public virtual void PerformAttack()
    {

    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void HandleFlip(float xVelocity)
    {
        if (facingRight && xVelocity < 0)
            Flip();
        else if (!facingRight && xVelocity > 0)
            Flip();
    }

    public void Flip()
    {
        facingRight = !facingRight;
        facingDir *= -1;
        transform.Rotate(0, 180, 0);
    }

    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        if (groundDetected)
            lastGroundedTime = Time.time;

        wallDetected = wallCheck != null && Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

        Vector2 aheadOrigin = (Vector2)groundCheck.position + Vector2.right * facingDir * ledgeCheckAheadDistance;
        ledgeAhead = !Physics2D.Raycast(aheadOrigin, Vector2.down, groundCheckDistance, whatIsGround);
    }

    public bool HasCoyoteGrounding() => Time.time - lastGroundedTime <= coyoteTime;

    public LayerMask GetWhatIsGround() => whatIsGround;

    protected virtual void OnDrawGizmos()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
        }

        Gizmos.color = Color.yellow;
        Vector3 aheadOrigin = groundCheck.position + new Vector3(ledgeCheckAheadDistance * facingDir, 0);
        Gizmos.DrawLine(aheadOrigin, aheadOrigin + new Vector3(0, -groundCheckDistance));
    }
}
