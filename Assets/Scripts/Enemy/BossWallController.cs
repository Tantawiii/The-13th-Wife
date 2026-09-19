using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

// Wraps the "Invisible Boss Wall" tilemap - fades its tiles out/in and
// enables/disables its collider(s) to match, so both the boss-gate opening
// (BossGateController) and the boss-area entry trap (BossAreaEntryTrigger)
// drive the same visual/physical toggle.
public class BossWallController : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [Tooltip("Whichever collider(s) actually block movement (usually just the CompositeCollider2D).")]
    [SerializeField] private Collider2D[] colliders;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine activeRoutine;

    private void Reset()
    {
        tilemap = GetComponent<Tilemap>();
        colliders = GetComponents<Collider2D>();
    }

    // open = true fades the wall away and disables its colliders.
    // open = false brings it back and re-enables them.
    public void SetOpen(bool open, bool instant = false)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        if (instant)
        {
            ApplyAlpha(open ? 0f : 1f);
            SetCollidersEnabled(!open);
            return;
        }

        activeRoutine = StartCoroutine(FadeCo(open));
    }

    private IEnumerator FadeCo(bool open)
    {
        if (!open)
            SetCollidersEnabled(true);

        float from = open ? 1f : 0f;
        float to = open ? 0f : 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            ApplyAlpha(Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / fadeDuration)));
            yield return null;
        }

        ApplyAlpha(to);
        SetCollidersEnabled(!open);
        activeRoutine = null;
    }

    private void ApplyAlpha(float alpha)
    {
        if (tilemap == null)
            return;

        Color color = tilemap.color;
        color.a = alpha;
        tilemap.color = color;
    }

    private void SetCollidersEnabled(bool value)
    {
        if (colliders == null)
            return;

        foreach (Collider2D col in colliders)
        {
            if (col != null)
                col.enabled = value;
        }
    }
}
