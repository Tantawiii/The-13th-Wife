using UnityEngine;

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
