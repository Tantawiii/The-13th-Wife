using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI_Credits : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    [SerializeField] private GameObject selfPanel;
    [SerializeField] private GameObject panelToReturnTo;

    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private float scrollSpeed = 60f;

    [SerializeField] private bool loopWhenFinished = true;
    [SerializeField] private bool returnToMainMenuWhenFinished = false;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [Tooltip("Level_0 context only - closes over everything right before the Main Menu load.")]
    [SerializeField] private UI_Curtain curtain;

    private InputAction cancelAction;
    private bool finished;

    private void Awake()
    {
        cancelAction = inputActions.FindActionMap("UI").FindAction("Cancel");
    }

    private void OnEnable()
    {
        finished = false;
        ResetScroll();

        inputActions.FindActionMap("UI").Enable();
        cancelAction.performed += OnCancelPressed;
    }

    private void OnDisable()
    {
        cancelAction.performed -= OnCancelPressed;
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        if (finished)
            return;

        if (returnToMainMenuWhenFinished)
        {
            finished = true;
            StartCoroutine(ReturnToMainMenuCo());
        }
        else
        {
            Back();
        }
    }

    private void ResetScroll()
    {
        if (content == null || viewport == null)
            return;

        float startY = -(viewport.rect.height * 0.5f + content.rect.height * 0.5f);
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, startY);
    }

    private void Update()
    {
        if (finished || content == null || viewport == null)
            return;

        content.anchoredPosition += new Vector2(0f, scrollSpeed * Time.deltaTime);

        float finishY = viewport.rect.height * 0.5f + content.rect.height * 0.5f;
        if (content.anchoredPosition.y < finishY)
            return;

        if (loopWhenFinished)
        {
            ResetScroll();
            return;
        }

        finished = true;

        if (returnToMainMenuWhenFinished)
            StartCoroutine(ReturnToMainMenuCo());
    }

    private IEnumerator ReturnToMainMenuCo()
    {
        if (curtain != null)
            yield return curtain.PlayClose();

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void Back()
    {
        if (selfPanel != null)
            selfPanel.SetActive(false);
        if (panelToReturnTo != null)
            panelToReturnTo.SetActive(true);
    }
}
