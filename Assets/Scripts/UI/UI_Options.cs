using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private const float UnselectedTabAlpha = 65f / 255f;

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

    public void CloseImmediately(bool isOn)
    {
        if (isOn)
            Back();
    }

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
        float volume = AudioVolume.LoadVolume(parameter);
        bool muted = AudioVolume.LoadMuted(parameter);

        if (slider != null)
            slider.SetValueWithoutNotify(volume);

        if (toggle != null)
            toggle.SetIsOnWithoutNotify(!muted);

        AudioVolume.Apply(audioMixer, parameter, volume, muted);
    }

    public void SetMasterVolume(float value)
    {
        AudioVolume.SaveVolume(masterParameter, value);
        AudioVolume.Apply(audioMixer, masterParameter, value, masterMuteToggle != null && !masterMuteToggle.isOn);
    }

    public void SetMasterMuted(bool isOn)
    {
        bool muted = !isOn;
        AudioVolume.SaveMuted(masterParameter, muted);
        float volume = masterVolumeSlider != null ? masterVolumeSlider.value : AudioVolume.LoadVolume(masterParameter);
        AudioVolume.Apply(audioMixer, masterParameter, volume, muted);
    }

    public void SetBGMVolume(float value)
    {
        AudioVolume.SaveVolume(bgmParameter, value);
        AudioVolume.Apply(audioMixer, bgmParameter, value, bgmMuteToggle != null && !bgmMuteToggle.isOn);
    }

    public void SetBGMMuted(bool isOn)
    {
        bool muted = !isOn;
        AudioVolume.SaveMuted(bgmParameter, muted);
        float volume = bgmVolumeSlider != null ? bgmVolumeSlider.value : AudioVolume.LoadVolume(bgmParameter);
        AudioVolume.Apply(audioMixer, bgmParameter, volume, muted);
    }

    public void SetSFXVolume(float value)
    {
        AudioVolume.SaveVolume(sfxParameter, value);
        AudioVolume.Apply(audioMixer, sfxParameter, value, sfxMuteToggle != null && !sfxMuteToggle.isOn);
    }

    public void SetSFXMuted(bool isOn)
    {
        bool muted = !isOn;
        AudioVolume.SaveMuted(sfxParameter, muted);
        float volume = sfxVolumeSlider != null ? sfxVolumeSlider.value : AudioVolume.LoadVolume(sfxParameter);
        AudioVolume.Apply(audioMixer, sfxParameter, volume, muted);
    }
}
