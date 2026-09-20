using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Boot : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        SceneManager.LoadSceneAsync(mainMenuSceneName);
    }
}
