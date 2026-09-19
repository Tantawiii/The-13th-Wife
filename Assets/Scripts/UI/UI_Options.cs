using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private const float UnselectedTabAlpha = 65f / 255f;
    private const float MixerMultiplier = 25f;
    private const float MutedDb = -80f;
    private const string MutedKeySuffix = "_Muted";

    [Header("Navigation")]
    [SerializeField] private GameObject selfPanel;
    [SerializeField] private GameObject panelToReturnTo;

    [Header("Tabs")]
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private TextMeshProUGUI controlsTabText;
    [SerializeField] private TextMeshProUGUI audioTabText;
    [Tooltip("Selected whenever its tab opens, so gamepad navigation always has a starting point.")]
    [SerializeField] private Button controlsTabButton;
    [SerializeField] private Button audioTabButton;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Tooltip("Master controls both Background Music and Sound Effects at once - it drives the AudioMixer's top-level Master group, which both of those route through, so it never fights with the individual sliders' saved values.")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Toggle masterMuteToggle;
    [SerializeField] private string masterParameter = "masterMixer";

    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Toggle bgmMuteToggle;
    [SerializeField] private string bgmParameter = "bgmMixer";

    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle sfxMuteToggle;
    [SerializeField] private string sfxParameter = "sfxMixer";

    private void OnEnable()
    {
        OpenControlsTab();
        LoadAudioSettings();
    }

    public void OpenControlsTab()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(true);
        if (audioPanel != null)
            audioPanel.SetActive(false);

        SetTabAlpha(controlsTabText, 1f);
        SetTabAlpha(audioTabText, UnselectedTabAlpha);

        if (controlsTabButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(controlsTabButton.gameObject);
    }

    public void OpenAudioTab()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
        if (audioPanel != null)
            audioPanel.SetActive(true);

        SetTabAlpha(controlsTabText, UnselectedTabAlpha);
        SetTabAlpha(audioTabText, 1f);

        if (audioTabButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(audioTabButton.gameObject);
    }

    private void SetTabAlpha(TextMeshProUGUI text, float alpha)
    {
        if (text == null)
            return;

        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    // Wired to each panel's close-toggle OnValueChanged.
    public void CloseImmediately(bool isOn)
    {
        if (isOn)
            Back();
    }

    // Generic close-and-return, reused by both the Main Menu's Options panel
    // and the in-game Pause menu's Options panel - whichever panel opened
    // this one is what gets reactivated.
    public void Back()
    {
        if (selfPanel != null)
            selfPanel.SetActive(false);
        if (panelToReturnTo != null)
            panelToReturnTo.SetActive(true);
    }

    private void LoadAudioSettings()
    {
        LoadChannel(masterVolumeSlider, masterMuteToggle, masterParameter);
        LoadChannel(bgmVolumeSlider, bgmMuteToggle, bgmParameter);
        LoadChannel(sfxVolumeSlider, sfxMuteToggle, sfxParameter);
    }

    private void LoadChannel(Slider slider, Toggle toggle, string parameter)
    {
        float volume = PlayerPrefs.GetFloat(parameter, .6f);
        bool muted = PlayerPrefs.GetInt(parameter + MutedKeySuffix, 0) == 1;

        // Set without notifying - loading saved state shouldn't itself count
        // as a user edit that re-writes PlayerPrefs.
        if (slider != null)
            slider.SetValueWithoutNotify(volume);

        // Toggle is "on" = unmuted, so it always reads as the enabled checkbox.
        if (toggle != null)
            toggle.SetIsOnWithoutNotify(!muted);

        ApplyToMixer(parameter, volume, muted);
    }

    private void ApplyToMixer(string parameter, float volume, bool muted)
    {
        if (audioMixer == null)
            return;

        float dB = muted ? MutedDb : Mathf.Log10(Mathf.Max(volume, 0.0001f)) * MixerMultiplier;
        audioMixer.SetFloat(parameter, dB);
    }

    // Wire to the Master slider's OnValueChanged in the Inspector.
    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat(masterParameter, value);
        ApplyToMixer(masterParameter, value, masterMuteToggle != null && !masterMuteToggle.isOn);
    }

    // Wire to the Master toggle's OnValueChanged in the Inspector.
    public void SetMasterMuted(bool isOn)
    {
        bool muted = !isOn;
        PlayerPrefs.SetInt(masterParameter + MutedKeySuffix, muted ? 1 : 0);
        float volume = masterVolumeSlider != null ? masterVolumeSlider.value : PlayerPrefs.GetFloat(masterParameter, .6f);
        ApplyToMixer(masterParameter, volume, muted);
    }

    // Wire to the BGM slider's OnValueChanged in the Inspector.
    public void SetBGMVolume(float value)
    {
        PlayerPrefs.SetFloat(bgmParameter, value);
        ApplyToMixer(bgmParameter, value, bgmMuteToggle != null && !bgmMuteToggle.isOn);
    }

    // Wire to the BGM toggle's OnValueChanged in the Inspector.
    public void SetBGMMuted(bool isOn)
    {
        bool muted = !isOn;
        PlayerPrefs.SetInt(bgmParameter + MutedKeySuffix, muted ? 1 : 0);
        float volume = bgmVolumeSlider != null ? bgmVolumeSlider.value : PlayerPrefs.GetFloat(bgmParameter, .6f);
        ApplyToMixer(bgmParameter, volume, muted);
    }

    // Wire to the SFX slider's OnValueChanged in the Inspector.
    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(sfxParameter, value);
        ApplyToMixer(sfxParameter, value, sfxMuteToggle != null && !sfxMuteToggle.isOn);
    }

    // Wire to the SFX toggle's OnValueChanged in the Inspector.
    public void SetSFXMuted(bool isOn)
    {
        bool muted = !isOn;
        PlayerPrefs.SetInt(sfxParameter + MutedKeySuffix, muted ? 1 : 0);
        float volume = sfxVolumeSlider != null ? sfxVolumeSlider.value : PlayerPrefs.GetFloat(sfxParameter, .6f);
        ApplyToMixer(sfxParameter, volume, muted);
    }
}
