using UnityEngine;

public class Checkpoint : MonoBehaviour, ISaveable
{
    [SerializeField] private string checkpointID;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource fireAudioSource;

    private bool isActive;

    // Tracks which checkpoint was touched most recently THIS session, so
    // SaveData knows whose position to write as the respawn point even though
    // FindObjectsByType's iteration order isn't touch-chronological. Stored by
    // ID (not a direct reference) so it round-trips correctly through save/load.
    private static string lastActivatedID;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(checkpointID))
            checkpointID = System.Guid.NewGuid().ToString();
#endif
    }

    private Vector3 GetRespawnPosition() => respawnPoint != null ? respawnPoint.position : transform.position;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        lastActivatedID = checkpointID;
        SetActive(true);
    }

    private void SetActive(bool active)
    {
        isActive = active;

        if (anim != null)
            anim.SetBool("isActive", active);

        if (fireAudioSource != null)
        {
            if (active && !fireAudioSource.isPlaying)
                fireAudioSource.Play();
            else if (!active)
                fireAudioSource.Stop();
        }
    }

    public void SaveData(ref GameData data)
    {
        if (!isActive)
            return;

        data.unlockedCheckpoints[checkpointID] = true;

        if (checkpointID == lastActivatedID)
        {
            data.hasCheckpoint = true;
            data.lastCheckpointPosition = GetRespawnPosition();
        }
    }

    public void LoadData(GameData data)
    {
        if (data.unlockedCheckpoints.TryGetValue(checkpointID, out bool unlocked) && unlocked)
            SetActive(true);
    }
}
