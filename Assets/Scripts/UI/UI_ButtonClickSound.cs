using UnityEngine;
using UnityEngine.UI;

// Wires a click SFX onto every Button under this Transform (including
// inactive ones, e.g. panels that start closed) at Awake, so it doesn't need
// to be hooked into each button's OnClick by hand. Attach once to a scene's
// root Canvas to cover every button in it.
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
