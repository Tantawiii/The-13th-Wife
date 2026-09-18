using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private CanvasGroup menuContentGroup;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private string gameplaySceneName = "Level_0";
    [SerializeField] private float revealDuration = 1f;
    [SerializeField] private float contentFadeDuration = 0.5f;
    [SerializeField] private float tilingDuration = 1f;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Continue Button")]
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI continueButtonText;
    [SerializeField] private float disabledAlpha = 65f / 255f;

    private void Awake()
    {
        if (continueButtonText == null && continueButton != null)
            continueButtonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();

        SetContinueButtonState(new FileDataHandler(Application.persistentDataPath, SaveManager.SaveFileName, true).SaveExists());

        // Pinned invisible immediately so there's no flash of the buttons
        // before the background's own reveal (fade + Tiling settle) finishes.
        if (menuContentGroup != null)
        {
            menuContentGroup.alpha = 0f;
            menuContentGroup.interactable = false;
            menuContentGroup.blocksRaycasts = false;
        }
    }

    private void Start()
    {
        StartCoroutine(RevealMenuCo());
    }

    private IEnumerator RevealMenuCo()
    {
        if (fadeScreen != null)
            yield return fadeScreen.RevealMainMenu(revealDuration);

        if (menuContentGroup == null)
            yield break;

        menuContentGroup.interactable = true;
        menuContentGroup.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < contentFadeDuration)
        {
            elapsed += Time.deltaTime;
            menuContentGroup.alpha = Mathf.Clamp01(elapsed / contentFadeDuration);
            yield return null;
        }

        menuContentGroup.alpha = 1f;
    }

    private void SetContinueButtonState(bool hasSave)
    {
        if (continueButton != null)
            continueButton.interactable = hasSave;

        if (continueButtonText != null)
        {
            Color color = continueButtonText.color;
            color.a = hasSave ? 1f : disabledAlpha;
            continueButtonText.color = color;
        }
    }

    public void NewGame()
    {
        new FileDataHandler(Application.persistentDataPath, SaveManager.SaveFileName, true).DeleteData();
        StartCoroutine(FadeAndLoad());
    }

    public void Continue()
    {
        StartCoroutine(FadeAndLoad());
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private IEnumerator FadeAndLoad()
    {
        // Grain up first while the menu is still fully visible, then (only
        // once that finishes) cover the screen for the scene load.
        if (fadeScreen != null)
            yield return fadeScreen.TransitionToGameplay(tilingDuration, fadeDuration);

        SceneManager.LoadScene(gameplaySceneName);
    }
}
