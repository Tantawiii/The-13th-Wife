using UnityEngine;

// Minimal on purpose: nothing in the project plays sounds by name yet, so
// RPG2D's full named-clip database (AudioDatabase_DataSO) would just be
// unused scaffolding right now. These two methods are enough to hang future
// calls off of; the Options menu's BGM/SFX sliders control these sources'
// output via the AudioMixer groups assigned on their AudioSource components.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}
