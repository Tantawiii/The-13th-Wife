using UnityEngine;
using UnityEngine.Audio;

public static class AudioVolume
{
    private const float MixerMultiplier = 25f;
    private const float MutedDb = -80f;
    private const string MutedKeySuffix = "_Muted";
    public const float DefaultVolume = 0.6f;

    public static float LoadVolume(string parameter) => PlayerPrefs.GetFloat(parameter, DefaultVolume);
    public static bool LoadMuted(string parameter) => PlayerPrefs.GetInt(parameter + MutedKeySuffix, 0) == 1;

    public static void SaveVolume(string parameter, float value)
    {
        PlayerPrefs.SetFloat(parameter, value);
        PlayerPrefs.Save();
    }

    public static void SaveMuted(string parameter, bool muted)
    {
        PlayerPrefs.SetInt(parameter + MutedKeySuffix, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void Apply(AudioMixer mixer, string parameter, float volume, bool muted)
    {
        if (mixer == null)
            return;

        float dB = muted ? MutedDb : Mathf.Log10(Mathf.Max(volume, 0.0001f)) * MixerMultiplier;
        mixer.SetFloat(parameter, dB);
    }

    public static void ApplySaved(AudioMixer mixer, string parameter)
    {
        Apply(mixer, parameter, LoadVolume(parameter), LoadMuted(parameter));
    }
}
