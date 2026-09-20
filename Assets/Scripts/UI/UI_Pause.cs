using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Pause : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float fadeDuration = 1f;
    [Tooltip("Selected whenever the pause panel opens, so gamepad navigation always has a starting point.")]
    [SerializeField] private Button resumeButton;

    private InputAction pauseAction;
    public bool isPaused { get; private set; }

    public static bool IsPaused { get; private set; }

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
        IsPaused = true;
        Time.timeScale = 0f;
        inputActions.FindActionMap("Player").Disable();

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (resumeButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void Resume()
    {
        isPaused = false;
        IsPaused = false;
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

        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;
        AudioManager.Instance?.StopAmbient();

        if (fadeScreen != null)
            yield return fadeScreen.FadeToBlack(fadeDuration);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
