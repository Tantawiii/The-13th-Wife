using System.Collections.Generic;
using UnityEngine;

// Reuses Enemy instances instead of Instantiate/Destroy on every spawn/death.
// Holds one sub-pool per prefab in enemyPrefabs, so several different enemy
// types can share the same pool and spawner. Each prefab needs a real prefab
// asset (drag the GameObject from the Hierarchy into a project folder) - it
// can't clone a lone scene instance safely once that instance itself gets
// pooled and reused.
// Shadya is a hand-placed boss, not a random spawn - leave her out of this list.
public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy[] enemyPrefabs;

    private readonly Dictionary<Enemy, Queue<Enemy>> pools = new Dictionary<Enemy, Queue<Enemy>>();
    private readonly Dictionary<Enemy, Enemy> prefabByInstance = new Dictionary<Enemy, Enemy>();

    public int ActiveCount { get; private set; }

    // Picks one of enemyPrefabs at random and gets/creates a pooled instance of it.
    public Enemy GetRandom(Vector3 position)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return null;

        Enemy prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        return Get(prefab, position);
    }

    public Enemy Get(Enemy prefab, Vector3 position)
    {
        Queue<Enemy> queue = GetOrCreateQueue(prefab);
        Enemy enemy = queue.Count > 0 ? queue.Dequeue() : CreateInstance(prefab);

        enemy.transform.position = position;
        enemy.gameObject.SetActive(true);
        ActiveCount++;
        return enemy;
    }

    public void Release(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        ActiveCount--;

        if (!prefabByInstance.TryGetValue(enemy, out Enemy prefab))
            return;

        GetOrCreateQueue(prefab).Enqueue(enemy);
    }

    private Enemy CreateInstance(Enemy prefab)
    {
        Enemy instance = Instantiate(prefab, transform);
        prefabByInstance[instance] = prefab;
        return instance;
    }

    private Queue<Enemy> GetOrCreateQueue(Enemy prefab)
    {
        if (!pools.TryGetValue(prefab, out Queue<Enemy> queue))
        {
            queue = new Queue<Enemy>();
            pools[prefab] = queue;
        }

        return queue;
    }
}
