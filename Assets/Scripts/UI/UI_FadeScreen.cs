using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum FadeScreenContext { None, MainMenu, Gameplay, Boot }

[RequireComponent(typeof(Image))]
public class UI_FadeScreen : MonoBehaviour
{
    [SerializeField] private Material fadeMaterial;
    [SerializeField] private bool fadeInOnStart = true;
    [SerializeField] private float startFadeDuration = 1f;

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

        if (fadeMaterial == null || context == FadeScreenContext.None)
            return;

        SetTilingImmediate(gameplayTilingValue);
        SetCoverImmediate(1f);
    }

    private void Start()
    {
        if (fadeMaterial == null || context == FadeScreenContext.None)
            return;

        if (context == FadeScreenContext.Boot || !fadeInOnStart)
            return;

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
