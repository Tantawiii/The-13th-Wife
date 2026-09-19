using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Periodically pulls a pooled enemy and "draws" it onto a ground platform -
// a brush sprite sweeps across the spawn point while the enemy fades in
// underneath, Looney Tunes style, before it's actually turned loose.
public class EnemySpawner : MonoBehaviour
{
    private static readonly List<EnemySpawner> activeSpawners = new List<EnemySpawner>();

    [Header("Pool")]
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private int maxActiveEnemies = 5;

    [Header("Spawn Area")]
    [Tooltip("Follows this Transform's X position if set (e.g. the Player), so enemies keep appearing near wherever they currently are instead of a fixed spot. Falls back to this spawner's own position if left empty.")]
    [SerializeField] private Transform followTarget;
    [Tooltip("Random X offset from the follow target (or this spawner's own position) to try spawning at.")]
    [SerializeField] private float spawnRangeX = 10f;
    [SerializeField] private float raycastStartHeight = 20f;
    [SerializeField] private float raycastDistance = 40f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Timing")]
    [Tooltip("Kept close together on purpose - a wide min/max gap makes the pacing feel random rather than deliberate.")]
    [SerializeField] private float minSpawnInterval = 3f;
    [SerializeField] private float maxSpawnInterval = 5f;

    [Header("Brush Draw-In")]
    [Tooltip("A GameObject with a SpriteRenderer using the brush texture. Optional - the enemy still fades in without it. If it has a BrushSpawnPoint child marking the brush's tip, that point (not the prefab's own pivot) is what gets aligned to the ground spawn point.")]
    [SerializeField] private SpriteRenderer brushPrefab;
    [SerializeField] private float drawDuration = 0.5f;
    [SerializeField] private Vector2 brushStartOffset = new Vector2(-0.5f, 1.5f);
    [SerializeField] private Vector2 brushEndOffset = new Vector2(0.5f, -0.2f);
    [SerializeField] private AnimationCurve drawCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AudioClip spawnClip;

    private bool spawningStopped;
    private bool spawningPaused;

    private void OnEnable()
    {
        activeSpawners.Add(this);
    }

    private void OnDisable()
    {
        activeSpawners.Remove(this);
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    // Called once Shadya (or any boss) goes down - the run is over, nothing
    // else should keep appearing.
    public static void StopAllSpawning()
    {
        foreach (EnemySpawner spawner in activeSpawners)
            spawner.spawningStopped = true;
    }

    // Temporary hold (boss-gate timeline) - unlike StopAllSpawning, this is
    // expected to be lifted again via ResumeAllSpawning().
    public static void PauseAllSpawning()
    {
        foreach (EnemySpawner spawner in activeSpawners)
            spawner.spawningPaused = true;
    }

    public static void ResumeAllSpawning()
    {
        foreach (EnemySpawner spawner in activeSpawners)
            spawner.spawningPaused = false;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            if (spawningStopped || spawningPaused || enemyPool == null || enemyPool.ActiveCount >= maxActiveEnemies)
                continue;

            if (TryFindGroundPoint(out Vector3 point))
                StartCoroutine(SpawnEnemyCo(point));
        }
    }

    private bool TryFindGroundPoint(out Vector3 point)
    {
        float originX = followTarget != null ? followTarget.position.x : transform.position.x;
        float x = originX + Random.Range(-spawnRangeX, spawnRangeX);
        Vector2 origin = new Vector2(x, transform.position.y + raycastStartHeight);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raycastDistance, groundLayer);

        if (hit.collider != null)
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    private IEnumerator SpawnEnemyCo(Vector3 groundPoint)
    {
        Enemy enemy = enemyPool.GetRandom(groundPoint);
        if (enemy == null)
            yield break;

        // GetRandom only placed it at the raw ground point - lift it so its
        // own feet (not its transform origin) land on the surface.
        Vector3 spawnPosition = groundPoint;
        spawnPosition.y += enemy.GetFeetOffset();
        enemy.transform.position = spawnPosition;

        enemy.PrepareForDraw();
        AudioManager.Instance?.PlaySFX(spawnClip);

        SpriteRenderer brush = brushPrefab != null ? Instantiate(brushPrefab) : null;
        Vector3 tipLocalOffset = Vector3.zero;

        if (brush != null)
        {
            BrushSpawnPoint tip = brush.GetComponentInChildren<BrushSpawnPoint>();
            if (tip != null)
                tipLocalOffset = tip.transform.position - brush.transform.position;
        }

        Vector3 start = spawnPosition + (Vector3)brushStartOffset - tipLocalOffset;
        Vector3 end = spawnPosition + (Vector3)brushEndOffset - tipLocalOffset;

        if (brush != null)
            brush.transform.position = start;

        float elapsed = 0f;
        while (elapsed < drawDuration)
        {
            elapsed += Time.deltaTime;
            float t = drawCurve.Evaluate(Mathf.Clamp01(elapsed / drawDuration));

            if (brush != null)
                brush.transform.position = Vector3.Lerp(start, end, t);

            enemy.SetDrawProgress(t);
            yield return null;
        }

        if (brush != null)
            Destroy(brush.gameObject);

        enemy.FinishSpawn(enemyPool);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = followTarget != null ? followTarget.position : transform.position;

        Gizmos.color = Color.magenta;
        Vector3 left = origin + Vector3.left * spawnRangeX;
        Vector3 right = origin + Vector3.right * spawnRangeX;
        Gizmos.DrawLine(left, right);
        Gizmos.DrawLine(left, left + Vector3.up * raycastStartHeight);
        Gizmos.DrawLine(right, right + Vector3.up * raycastStartHeight);
        Gizmos.DrawLine(left + Vector3.up * raycastStartHeight, right + Vector3.up * raycastStartHeight);
    }
}
