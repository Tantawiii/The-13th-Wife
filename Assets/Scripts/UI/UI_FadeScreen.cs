using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Drives two things in lockstep: the project's shared "Full Film Clip Grain"
// material (applied screen-wide via a FullScreenPassRendererFeature on
// Renderer2D.asset, so its _LerpValue/_Tiling persist across scene loads),
// and a plain black Image sitting on this same GameObject - a belt-and-
// suspenders UI cover for the brief window during an actual scene load where
// the shader's renderer feature has no camera to apply to yet.
public enum FadeScreenContext { None, MainMenu, Gameplay, Boot }

[RequireComponent(typeof(Image))]
public class UI_FadeScreen : MonoBehaviour
{
    [SerializeField] private Material fadeMaterial;
    [SerializeField] private bool fadeInOnStart = true;
    [SerializeField] private float startFadeDuration = 1f;

    // Which scene this instance lives in. Every context pins Tiling to
    // gameplayTilingValue on start - Boot and Gameplay because that's simply
    // their look, Main Menu because its own reveal always eases down FROM
    // that value, regardless of whether the previous scene was Boot or
    // Gameplay (the shared material otherwise keeps whatever the last scene
    // left it at).
    [SerializeField] private FadeScreenContext context = FadeScreenContext.None;

    [Header("Tiling Look")]
    [Tooltip("Tiling value for the main menu's 'film roll' look.")]
    public float menuTilingValue = 0f;
    [Tooltip("Tiling value for the 'film grain' gameplay look.")]
    public float gameplayTilingValue = 1f;

    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    private void Start()
    {
        if (fadeMaterial == null || context == FadeScreenContext.None)
            return;

        SetTilingImmediate(gameplayTilingValue);
        SetCoverImmediate(1f);

        // Boot just holds the black cover - whatever loads Main Menu next
        // decides when it's time to leave.
        if (context == FadeScreenContext.Boot || !fadeInOnStart)
            return;

        // Main Menu's reveal is more involved (it also has to settle Tiling
        // and then fade its own button content in), so UI_MainMenu explicitly
        // orchestrates it via RevealMainMenu() instead of it self-triggering here.
        if (context == FadeScreenContext.Gameplay)
            StartCoroutine(FadeIn(startFadeDuration));
    }

    public void SetTilingImmediate(float value)
    {
        if (fadeMaterial != null)
            fadeMaterial.SetFloat("_Tiling", value);
    }

    private void SetCoverImmediate(float value)
    {
        SetCover(value);
    }

    // Keeps the shader blackout and the UI Image moving together - they
    // always represent the same "how covered is the screen" amount.
    private void SetCover(float value)
    {
        if (fadeMaterial != null)
            fadeMaterial.SetFloat("_LerpValue", value);

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = value;
            fadeImage.color = color;
        }
    }

    // Covers the screen (0 -> 1), Tiling untouched. Used leaving Gameplay for
    // the Main Menu, where the look should keep reading as gameplay grain
    // until the Main Menu's own reveal eases it back down.
    public IEnumerator FadeToBlack(float duration = 1f)
    {
        if (fadeMaterial == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetCover(Mathf.Lerp(0f, 1f, t));
            yield return null;
        }

        SetCover(1f);
    }

    // Uncovers the screen (1 -> 0), Tiling untouched. Used entering Gameplay,
    // where Tiling simply stays at the gameplay look throughout.
    public IEnumerator FadeIn(float duration = 1f)
    {
        if (fadeMaterial == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetCover(Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        SetCover(0f);
    }

    // Main Menu's own reveal, regardless of where it was loaded from (Boot or
    // Gameplay both leave Tiling pinned at gameplayTilingValue): uncovers the
    // screen while easing Tiling down to the menu look, at the same time.
    // Called explicitly by UI_MainMenu, which layers its own button-content
    // fade-in on top once this finishes.
    public IEnumerator RevealMainMenu(float duration)
    {
        if (fadeMaterial == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetCover(Mathf.Lerp(1f, 0f, t));
            fadeMaterial.SetFloat("_Tiling", Mathf.Lerp(gameplayTilingValue, menuTilingValue, t));
            yield return null;
        }

        SetCover(0f);
        fadeMaterial.SetFloat("_Tiling", menuTilingValue);
    }

    // Main Menu's own send-off into Gameplay: Tiling grains up first while
    // the menu is still fully visible, then (only once that finishes) the
    // screen covers over for the scene load.
    public IEnumerator TransitionToGameplay(float tilingDuration, float coverDuration)
    {
        if (fadeMaterial == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < tilingDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / tilingDuration);
            fadeMaterial.SetFloat("_Tiling", Mathf.Lerp(menuTilingValue, gameplayTilingValue, t));
            yield return null;
        }

        fadeMaterial.SetFloat("_Tiling", gameplayTilingValue);

        yield return FadeToBlack(coverDuration);
    }
}
