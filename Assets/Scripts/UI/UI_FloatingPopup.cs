using System.Collections;
using TMPro;
using UnityEngine;

// Generic brief on-screen message: fades in, holds, fades out. Used for the
// "+1 Life" bonus-life callout, reusable for anything similar later.
public class UI_FloatingPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Coroutine activeRoutine;

    public void Show(string message)
    {
        if (text != null)
            text.text = message;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(ShowCo());
    }

    private IEnumerator ShowCo()
    {
        yield return Fade(0f, 1f, fadeInDuration);

        yield return new WaitForSeconds(holdDuration);

        yield return Fade(1f, 0f, fadeOutDuration);

        activeRoutine = null;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (group == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        group.alpha = to;
    }
}
