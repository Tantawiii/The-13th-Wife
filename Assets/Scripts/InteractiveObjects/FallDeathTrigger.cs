using UnityEngine;

// A wide trigger placed below the level's lowest reachable point (needs a
// BoxCollider2D with Is Trigger on, stretched across the level width) -
// anything that falls into it either loses (Player) or is scored and pooled
// (Enemy). Mirrors the fall-death pit from
// github.com/Tantawiii/Endless-Runner.
[RequireComponent(typeof(Collider2D))]
public class FallDeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.Die();
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
            enemy.FallOut();
    }
}
