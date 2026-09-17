using System.Collections;
using UnityEngine;

// Drives the project's existing "Full Film Clip Grain" material, which is
// already applied screen-wide via a FullScreenPassRendererFeature on
// Renderer2D.asset - no scene quad/Image needed, just this Material reference.
public enum FadeScreenContext { None, MainMenu, Gameplay }

public class UI_FadeScreen : MonoBehaviour
{
    [SerializeField] private Material fadeMaterial;
    [SerializeField] private bool fadeInOnStart = true;
    [SerializeField] private float startFadeDuration = 1f;

    // Which scene this instance lives in, so Tiling is always pinned to the
    // right value on start regardless of whatever the material asset (a
    // project-wide resource, not scene-local state) was last left at - e.g.
    // pressing Play directly on the gameplay scene without going through the
    // menu must still show Tiling = 1, not whatever the editor left it at.
    [SerializeField] private FadeScreenContext context = FadeScreenContext.None;

    [Header("Tiling Look")]
    [Tooltip("Tiling value for the main menu's 'film roll' look.")]
    public float menuTilingValue = 0f;
    [Tooltip("Tiling value for the 'film grain' gameplay look, reached once, during FadeOut.")]
    public float gameplayTilingValue = 1f;

    private void Start()
    {
        if (fadeMaterial != null)
        {
            if (context == FadeScreenContext.MainMenu)
                SetTilingImmediate(menuTilingValue);
            else if (context == FadeScreenContext.Gameplay)
                SetTilingImmediate(gameplayTilingValue);
        }

        if (!fadeInOnStart || fadeMaterial == null)
            return;

        // Force black immediately so there's no flash of whatever the material
        // asset's _LerpValue was last left at, then animate the blackout away.
        fadeMaterial.SetFloat("_LerpValue", 1f);
        StartCoroutine(FadeIn(startFadeDuration));
    }

    public void SetTilingImmediate(float value)
    {
        if (fadeMaterial != null)
            fadeMaterial.SetFloat("_Tiling", value);
    }

    // Blacks the screen out while shifting the film-roll look toward the
    // film-grain gameplay look - used once, leaving the main menu.
    public IEnumerator FadeOut(float duration = 1f)
    {
        if (fadeMaterial == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            fadeMaterial.SetFloat("_LerpValue", Mathf.Lerp(0f, 1f, t));
            fadeMaterial.SetFloat("_Tiling", Mathf.Lerp(menuTilingValue, gameplayTilingValue, t));
            yield return null;
        }

        fadeMaterial.SetFloat("_LerpValue", 1f);
        fadeMaterial.SetFloat("_Tiling", gameplayTilingValue);
    }

    // Lifts the blackout only - Tiling is left wherever it already is (the
    // gameplay "film grain" look established by FadeOut persists).
    public IEnumerator FadeIn(float duration = 1f)
    {
        if (fadeMaterial == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            fadeMaterial.SetFloat("_LerpValue", Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        fadeMaterial.SetFloat("_LerpValue", 0f);
    }
}
