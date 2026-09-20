using UnityEngine;

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
