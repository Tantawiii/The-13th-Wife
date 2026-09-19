using UnityEngine;
using UnityEngine.SceneManagement;

// Auto-scrolls creditsContent upward inside viewport (which should have a
// RectMask2D so it clips cleanly). Reusable in two contexts: opened from the
// Main Menu (loops forever, Back button returns to it), or played once after
// beating Shadya (returnToMainMenuWhenFinished, no loop, no Back button needed).
public class UI_Credits : MonoBehaviour
{
    [SerializeField] private GameObject selfPanel;
    [SerializeField] private GameObject panelToReturnTo;

    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private float scrollSpeed = 60f;

    [SerializeField] private bool loopWhenFinished = true;
    [SerializeField] private bool returnToMainMenuWhenFinished = false;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool finished;

    private void OnEnable()
    {
        finished = false;
        ResetScroll();
    }

    private void ResetScroll()
    {
        if (content == null || viewport == null)
            return;

        // Content's top edge starts just below the viewport's bottom edge.
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
            SceneManager.LoadScene(mainMenuSceneName);
    }

    // Wired to the Credits panel's own Back button (Main Menu context only).
    public void Back()
    {
        if (selfPanel != null)
            selfPanel.SetActive(false);
        if (panelToReturnTo != null)
            panelToReturnTo.SetActive(true);
    }
}
