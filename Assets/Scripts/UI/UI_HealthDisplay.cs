using UnityEngine;
using UnityEngine.UI;

// Drives the heart icons (each worth 2 hits) from Player's current hits taken
// vs current max hits. Hearts beyond the current max are hidden entirely -
// maxHits can grow at runtime via Player.GrantBonusLife, up to Player.MaxPossibleHits.
public class UI_HealthDisplay : MonoBehaviour
{
    [SerializeField] private Player player;
    [Tooltip("Each heart's root object (its outline/background), left to right.")]
    [SerializeField] private GameObject[] heartRoots;
    [Tooltip("Each heart's fill Image (Filled/Horizontal), matching heartRoots by index.")]
    [SerializeField] private Image[] heartFills;

    private void OnEnable()
    {
        if (player != null)
            player.OnHealthChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnHealthChanged -= Refresh;
    }

    private void Refresh()
    {
        if (player == null)
            return;

        int hitsTaken = player.HitsTaken;
        int maxHits = player.maxHits;

        for (int i = 0; i < heartRoots.Length; i++)
        {
            int heartCapacity = Mathf.Clamp(maxHits - i * 2, 0, 2);
            bool unlocked = heartCapacity > 0;

            if (heartRoots[i] != null)
                heartRoots[i].SetActive(unlocked);

            if (!unlocked || heartFills[i] == null)
                continue;

            int hitsOnThisHeart = Mathf.Clamp(hitsTaken - i * 2, 0, heartCapacity);
            heartFills[i].fillAmount = (heartCapacity - hitsOnThisHeart) / 2f;
        }
    }
}
