using UnityEngine;

// Drop this on whichever child Transform in the brush prefab marks the exact
// tip of the brush art - EnemySpawner offsets the whole brush so this point
// (not the prefab's own pivot) is what lines up with the ground spawn point.
public class BrushSpawnPoint : MonoBehaviour
{
}
