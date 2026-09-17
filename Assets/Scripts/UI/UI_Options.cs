using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private const float UnselectedTabAlpha = 65f / 255f;
    private const float MixerMultiplier = 25f;

    [Header("Navigation")]
    [SerializeField] private GameObject selfPanel;
    [SerializeField] private GameObject panelToReturnTo;

    [Header("Tabs")]
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private TextMeshProUGUI controlsTabText;
    [SerializeField] private TextMeshProUGUI audioTabText;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private string bgmParameter = "bgmMixer";
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private string sfxParameter = "sfxMixer";

    private void OnEnable()
    {
        OpenControlsTab();
        LoadVolume();
    }

    public void OpenControlsTab()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(true);
        if (audioPanel != null)
            audioPanel.SetActive(false);

        SetTabAlpha(controlsTabText, 1f);
        SetTabAlpha(audioTabText, UnselectedTabAlpha);
    }

    public void OpenAudioTab()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
        if (audioPanel != null)
            audioPanel.SetActive(true);

        SetTabAlpha(controlsTabText, UnselectedTabAlpha);
        SetTabAlpha(audioTabText, 1f);
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

    private void LoadVolume()
    {
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = PlayerPrefs.GetFloat(bgmParameter, .6f);
            SetBGMVolume(bgmVolumeSlider.value);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat(sfxParameter, .6f);
            SetSFXVolume(sfxVolumeSlider.value);
        }
    }

    // Wire to the BGM slider's OnValueChanged in the Inspector.
    public void SetBGMVolume(float value)
    {
        if (audioMixer != null)
            audioMixer.SetFloat(bgmParameter, Mathf.Log10(Mathf.Max(value, 0.0001f)) * MixerMultiplier);

        PlayerPrefs.SetFloat(bgmParameter, value);
    }

    // Wire to the SFX slider's OnValueChanged in the Inspector.
    public void SetSFXVolume(float value)
    {
        if (audioMixer != null)
            audioMixer.SetFloat(sfxParameter, Mathf.Log10(Mathf.Max(value, 0.0001f)) * MixerMultiplier);

        PlayerPrefs.SetFloat(sfxParameter, value);
    }
}
