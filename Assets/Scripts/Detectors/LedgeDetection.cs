using System.Collections;
using UnityEngine;

// Sits on a child object positioned out in front of the Player at ledge
// height. Ledge validity is just "is the ground layer within radius here" -
// shaping/placement of the ledge is handled by level design, not by this
// component (ported from Endless-Runner's LedgeDetection.cs).
public class LedgeDetection : MonoBehaviour
{
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private LayerMask whatIsGround;

    public bool ledgeDetected { get; private set; }
    public bool canGrabLedge { get; private set; } = true;

    private Coroutine reenableCoroutine;

    private void Update()
    {
        ledgeDetected = canGrabLedge && Physics2D.OverlapCircle(transform.position, radius, whatIsGround);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsGround(collision))
            SetCanGrabLedge(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsGround(collision))
            SetCanGrabLedge(true);
    }

    private bool IsGround(Collider2D collision) => ((1 << collision.gameObject.layer) & whatIsGround) != 0;

    public void SetCanGrabLedge(bool value) => canGrabLedge = value;

    public void ReenableAfterDelay(float delay)
    {
        if (reenableCoroutine != null)
            StopCoroutine(reenableCoroutine);

        reenableCoroutine = StartCoroutine(ReenableCo(delay));
    }

    private IEnumerator ReenableCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        canGrabLedge = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
