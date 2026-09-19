using UnityEngine;

// Starts Level_0's music and ambient film-grain noise loop on load. Left
// running across Pause/Options/Lose since none of those stop time for audio -
// only actually stopped when leaving the level entirely (see UI_Pause and
// UI_LevelLose's return-to-menu paths, which call AudioManager.StopAmbient()
// before loading MainMenu).
public class LevelAudio : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip ambientClip;

    private void Start()
    {
        AudioManager.Instance?.PlayBGM(musicClip);
        AudioManager.Instance?.PlayAmbient(ambientClip);
    }
}
