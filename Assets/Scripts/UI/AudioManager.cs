using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Audio Mixer")]
    [Tooltip("Applied from whatever's saved in PlayerPrefs (or the .6 build default) right before the very first sound plays - so volume/mute is already correct from the first note of BGM, not just once Options is opened.")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterParameter = "masterMixer";
    [SerializeField] private string bgmParameter = "bgmMixer";
    [SerializeField] private string sfxParameter = "sfxMixer";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureExists()
    {
        if (Instance != null)
            return;

        GameObject prefab = Resources.Load<GameObject>("AudioManager");

        if (prefab != null)
            Instantiate(prefab);
    }

    private bool volumeApplied;

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

    private void EnsureVolumeApplied()
    {
        if (volumeApplied)
            return;

        volumeApplied = true;
        AudioVolume.ApplySaved(audioMixer, masterParameter);
        AudioVolume.ApplySaved(audioMixer, bgmParameter);
        AudioVolume.ApplySaved(audioMixer, sfxParameter);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null)
            return;

        EnsureVolumeApplied();
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        EnsureVolumeApplied();
        sfxSource.PlayOneShot(clip);
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (ambientSource == null || clip == null)
            return;

        EnsureVolumeApplied();
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
