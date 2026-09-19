using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

// Watches ScoreManager's kill count - once the player has downed enough
// enemies, opens the Invisible Boss Wall with a short cinematic beat (camera
// cut onto the boss via an optional Timeline while the wall fades away,
// enemies frozen and spawning paused throughout) before handing control back
// to the player.
public class BossGateController : MonoBehaviour
{
    [SerializeField] private int enemiesRequiredToOpenGate = 12;
    [SerializeField] private BossWallController bossWall;
    [SerializeField] private UI_TypewriterText typewriter;
    [TextArea]
    [SerializeField] private string gateOpenLine = "Survive the latest wive, good luck";
    [Tooltip("Optional - plays a camera cut onto the boss while the wall fades. If left empty (or has no Timeline asset assigned yet), the freeze/pause window falls back to fallbackCinematicDuration instead of waiting on it.")]
    [SerializeField] private PlayableDirector cinematicDirector;
    [SerializeField] private float fallbackCinematicDuration = 3f;

    private bool gateOpened;

    private void Update()
    {
        if (gateOpened)
            return;

        if (ScoreManager.Instance != null && ScoreManager.Instance.EnemiesDefeated >= enemiesRequiredToOpenGate)
        {
            gateOpened = true;
            StartCoroutine(OpenGateCo());
        }
    }

    private IEnumerator OpenGateCo()
    {
        EnemySpawner.PauseAllSpawning();
        SetAllEnemiesFrozen(true);

        bossWall?.SetOpen(true);

        if (cinematicDirector != null && cinematicDirector.playableAsset != null)
        {
            cinematicDirector.Play();
            yield return new WaitForSeconds((float)cinematicDirector.duration);
        }
        else
        {
            yield return new WaitForSeconds(fallbackCinematicDuration);
        }

        SetAllEnemiesFrozen(false);
        EnemySpawner.ResumeAllSpawning();

        typewriter?.Play(gateOpenLine);
    }

    private void SetAllEnemiesFrozen(bool value)
    {
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            enemy.SetFrozen(value);
    }
}
