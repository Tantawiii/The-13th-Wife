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
    [SerializeField] private AudioSource ambientSource;

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

    // Always loops - there's no case in this game where BGM should play once
    // and stop.
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    // Separate looping channel from bgmSource - lets an ambient layer (e.g.
    // the in-game film grain noise) run underneath the music independently.
    public void PlayAmbient(AudioClip clip)
    {
        if (ambientSource == null || clip == null)
            return;

        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public void StopAmbient()
    {
        if (ambientSource != null)
            ambientSource.Stop();
    }
}
