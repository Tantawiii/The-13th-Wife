using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Shown on Player death. Time.timeScale is left alone on purpose - unlike the
// Pause menu, the level keeps running behind this screen.
public class UI_LevelLose : MonoBehaviour
{
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private float fadeDuration = 1f;

    [Tooltip("HUD elements (health hearts, score) to fade out alongside the screen fade, so they don't sit on top of it.")]
    [SerializeField] private CanvasGroup healthGroup;
    [SerializeField] private CanvasGroup scoreGroup;

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
        StartCoroutine(FadeOutHudCo(fadeDuration));

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

    private IEnumerator FadeOutHudCo(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / duration);

            if (healthGroup != null)
                healthGroup.alpha = alpha;
            if (scoreGroup != null)
                scoreGroup.alpha = alpha;

            yield return null;
        }

        if (healthGroup != null)
            healthGroup.alpha = 0f;
        if (scoreGroup != null)
            scoreGroup.alpha = 0f;
    }

    // Wired to Retry_BTN - restarts from whatever checkpoint was reached this
    // session (saved here so it survives the reload), or the level's own
    // default spawn if none was ever reached.
    public void Retry()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Wired to MainMenu_BTN - same checkpoint-or-level save as Retry, then
    // back to the Main Menu instead of reloading here. Screen is already
    // fully covered from ShowLoseScreenCo, so no fade needed before the load.
    public void SaveAndReturnToMenu()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        AudioManager.Instance?.StopAmbient();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
