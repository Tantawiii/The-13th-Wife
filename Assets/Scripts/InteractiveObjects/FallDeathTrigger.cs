using UnityEngine;

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
