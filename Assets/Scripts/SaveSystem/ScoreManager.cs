using UnityEngine;

// Tracks run progress: how many enemies have been defeated and how long the
// player has survived. Singleton per scene, mirroring SaveManager's own
// Instance pattern.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int EnemiesDefeated { get; private set; }
    public float SurvivalTime { get; private set; }
    public bool IsTracking { get; private set; } = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (IsTracking)
            SurvivalTime += Time.deltaTime;
    }

    public void RegisterEnemyDefeated()
    {
        EnemiesDefeated++;
    }

    // Called on Player death or on beating Shadya - the run is over either way.
    public void StopTracking()
    {
        IsTracking = false;
    }
}
