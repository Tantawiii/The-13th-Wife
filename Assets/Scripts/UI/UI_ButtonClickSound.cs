using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioClip clickClip;

    private void Awake()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
            button.onClick.AddListener(PlayClick);
    }

    private void PlayClick()
    {
        AudioManager.Instance?.PlaySFX(clickClip);
    }
}
