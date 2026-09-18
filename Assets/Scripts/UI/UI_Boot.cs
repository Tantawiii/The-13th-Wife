using UnityEngine;
using UnityEngine.SceneManagement;

// Boot stays on its pinned black cover (see UI_FadeScreen's Boot context)
// for as long as this scene is loaded - it has nothing to show. Loading Main
// Menu asynchronously here lets Unity finish its own startup costs (domain
// reload, first asset loads) behind that cover instead of on a hard cut.
public class UI_Boot : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        SceneManager.LoadSceneAsync(mainMenuSceneName);
    }
}
