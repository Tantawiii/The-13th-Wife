using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LevelLose : MonoBehaviour
{
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private UI_Curtain curtain;

    [Tooltip("HUD elements (health hearts, score, profile background) to fade out alongside the screen fade, so they don't sit on top of it.")]
    [SerializeField] private CanvasGroup healthGroup;
    [SerializeField] private CanvasGroup scoreGroup;
    [SerializeField] private CanvasGroup profileGroup;
    [Tooltip("The dialogue overlay - if a line is still showing when the player dies, it fades out with everything else instead of sitting on top of the lose screen.")]
    [SerializeField] private UI_TypewriterText dialogue;

    [SerializeField] private GameObject loseTextObject;
    [SerializeField] private CanvasGroup loseTextGroup;
    [SerializeField] private GameObject buttonsObject;
    [SerializeField] private CanvasGroup buttonsGroup;
    [SerializeField] private float contentFadeDuration = 0.5f;

    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [Tooltip("Selected once the reveal finishes, so gamepad navigation always has a starting point.")]
    [SerializeField] private Button retryButton;

    public void ShowLoseScreen()
    {
        StartCoroutine(ShowLoseScreenCo());
    }

    private IEnumerator ShowLoseScreenCo()
    {
        StartCoroutine(HudFader.FadeOut(fadeDuration, healthGroup, scoreGroup, profileGroup));
        dialogue?.FadeOutImmediately(fadeDuration);

        if (fadeScreen != null)
            yield return fadeScreen.FadeToBlack(fadeDuration);

        if (loseTextObject != null)
            loseTextObject.SetActive(true);
        if (buttonsObject != null)
            buttonsObject.SetActive(true);

        if (loseTextGroup != null)
            loseTextGroup.alpha = 0f;

        if (buttonsGroup != null)
        {
            buttonsGroup.alpha = 0f;
            buttonsGroup.interactable = false;
            buttonsGroup.blocksRaycasts = false;
        }

        float elapsed = 0f;
        while (elapsed < contentFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / contentFadeDuration);

            if (loseTextGroup != null)
                loseTextGroup.alpha = t;
            if (buttonsGroup != null)
                buttonsGroup.alpha = t;

            yield return null;
        }

        if (loseTextGroup != null)
            loseTextGroup.alpha = 1f;

        if (buttonsGroup != null)
        {
            buttonsGroup.alpha = 1f;
            buttonsGroup.interactable = true;
            buttonsGroup.blocksRaycasts = true;
        }

        if (retryButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(retryButton.gameObject);
    }

    public void Retry()
    {
        StartCoroutine(RetryCo());
    }

    private IEnumerator RetryCo()
    {
        if (curtain != null)
            yield return curtain.PlayClose();

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveAndReturnToMenu()
    {
        StartCoroutine(SaveAndReturnToMenuCo());
    }

    private IEnumerator SaveAndReturnToMenuCo()
    {
        if (curtain != null)
            yield return curtain.PlayClose();

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        AudioManager.Instance?.StopAmbient();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
