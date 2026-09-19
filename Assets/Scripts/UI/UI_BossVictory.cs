using System.Collections;
using UnityEngine;

// Called by Enemy.Die() when the boss (Shadya) goes down: "To Be Continued"
// holds for a few seconds, fades out, then the credits panel takes over and
// rolls once more before returning to the Main Menu.
public class UI_BossVictory : MonoBehaviour
{
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private GameObject toBeContinuedObject;
    [SerializeField] private CanvasGroup toBeContinuedGroup;
    [SerializeField] private float displayDuration = 6f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private GameObject creditsPanel;

    public void ShowVictory()
    {
        StartCoroutine(ShowVictoryCo());
    }

    private IEnumerator ShowVictoryCo()
    {
        AudioManager.Instance?.PlaySFX(victoryClip);

        if (toBeContinuedObject != null)
            toBeContinuedObject.SetActive(true);
        if (toBeContinuedGroup != null)
            toBeContinuedGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (toBeContinuedGroup != null)
                toBeContinuedGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        if (toBeContinuedObject != null)
            toBeContinuedObject.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }
}
