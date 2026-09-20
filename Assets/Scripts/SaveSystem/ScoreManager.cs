using UnityEngine;

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

    public void StopTracking()
    {
        IsTracking = false;
    }
}
