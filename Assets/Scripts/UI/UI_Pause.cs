using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Mirrors RPG2D's UI.cs pattern: the UI action map stays enabled at all times
// (so Cancel always works to open/close the pause menu regardless of whether
// gameplay is currently running); only the Player action map gets toggled
// off while paused, so the two can never end up both active (moving the
// character while a menu is open) or both inactive (stuck input) at once.
public class UI_Pause : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float fadeDuration = 1f;

    private InputAction pauseAction;
    public bool isPaused { get; private set; }

    private void Awake()
    {
        pauseAction = inputActions.FindActionMap("UI").FindAction("Cancel");
    }

    private void OnEnable()
    {
        inputActions.FindActionMap("UI").Enable();
        pauseAction.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePressed;
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        inputActions.FindActionMap("Player").Disable();

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);

        inputActions.FindActionMap("Player").Enable();
    }

    public void OpenOptions()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void SaveAndExit()
    {
        StartCoroutine(SaveAndExitCo());
    }

    private IEnumerator SaveAndExitCo()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        // Reset before leaving, not after - a lingering Time.timeScale of 0
        // would otherwise freeze the fade coroutine (and everything else) in
        // the Main Menu scene we're loading into.
        Time.timeScale = 1f;

        if (fadeScreen != null)
            yield return fadeScreen.FadeOut(fadeDuration);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
