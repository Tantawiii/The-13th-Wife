using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private string gameplaySceneName = "Level_0";
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
        if (fadeScreen != null)
            yield return fadeScreen.FadeOut(fadeDuration);

        SceneManager.LoadScene(gameplaySceneName);
    }
}
