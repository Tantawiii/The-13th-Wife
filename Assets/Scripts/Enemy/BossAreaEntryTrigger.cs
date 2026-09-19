using UnityEngine;

// Placed at the boss area's entrance. Once the player walks past it, the
// Invisible Boss Wall snaps back up behind them, sealing them in with the
// boss - one shot, then this trigger disables itself.
public class BossAreaEntryTrigger : MonoBehaviour
{
    [SerializeField] private BossWallController bossWall;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        bossWall?.SetOpen(false);
        gameObject.SetActive(false);
    }
}
