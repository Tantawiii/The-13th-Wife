using System.Collections;
using TMPro;
using UnityEngine;

// Reusable char-by-char dialogue overlay - fades in, types the message out,
// holds, fades out. Used for the level-intro line and the boss-gate line.
public class UI_TypewriterText : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float secondsPerCharacter = 0.04f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Coroutine activeRoutine;

    public void Play(string message, System.Action onComplete = null)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(PlayCo(message, onComplete));
    }

    private IEnumerator PlayCo(string message, System.Action onComplete)
    {
        if (text != null)
            text.text = string.Empty;

        yield return Fade(0f, 1f, fadeInDuration);

        if (text != null)
        {
            foreach (char c in message)
            {
                text.text += c;
                yield return new WaitForSeconds(secondsPerCharacter);
            }
        }

        yield return new WaitForSeconds(holdDuration);

        yield return Fade(1f, 0f, fadeOutDuration);

        activeRoutine = null;
        onComplete?.Invoke();
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
