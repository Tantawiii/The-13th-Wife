using System.Collections;
using UnityEngine;

public class UI_BossVictory : MonoBehaviour
{
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private GameObject toBeContinuedObject;
    [SerializeField] private CanvasGroup toBeContinuedGroup;
    [SerializeField] private float displayDuration = 6f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private GameObject creditsPanel;

    [Tooltip("HUD elements (health hearts, score, profile background) to fade out as \"To Be Continued\" appears, so they don't sit on top of it or the credits after.")]
    [SerializeField] private CanvasGroup healthGroup;
    [SerializeField] private CanvasGroup scoreGroup;
    [SerializeField] private CanvasGroup profileGroup;
    [Tooltip("The dialogue overlay - if a line is still showing when the boss dies, it fades out with everything else instead of sitting on top of \"To Be Continued\"/credits.")]
    [SerializeField] private UI_TypewriterText dialogue;

    public void ShowVictory()
    {
        StartCoroutine(ShowVictoryCo());
    }

    private IEnumerator ShowVictoryCo()
    {
        StartCoroutine(HudFader.FadeOut(fadeDuration, healthGroup, scoreGroup, profileGroup));
        dialogue?.FadeOutImmediately(fadeDuration);

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
