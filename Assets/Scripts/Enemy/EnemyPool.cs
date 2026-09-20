using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy[] enemyPrefabs;

    private readonly Dictionary<Enemy, Queue<Enemy>> pools = new Dictionary<Enemy, Queue<Enemy>>();
    private readonly Dictionary<Enemy, Enemy> prefabByInstance = new Dictionary<Enemy, Enemy>();

    public int ActiveCount { get; private set; }

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
