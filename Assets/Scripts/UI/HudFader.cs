using System.Collections;
using UnityEngine;

public static class HudFader
{
    public static IEnumerator FadeOut(float duration, params CanvasGroup[] groups)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / duration);

            foreach (CanvasGroup group in groups)
                if (group != null)
                    group.alpha = alpha;

            yield return null;
        }

        foreach (CanvasGroup group in groups)
            if (group != null)
                group.alpha = 0f;
    }
}
