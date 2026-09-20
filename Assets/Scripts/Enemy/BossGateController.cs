using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class BossGateController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private int enemiesRequiredToOpenGate = 12;
    [SerializeField] private BossWallController bossWall;
    [SerializeField] private UI_TypewriterText typewriter;
    [TextArea]
    [SerializeField] private string gateOpenLine = "Survive the latest wive, good luck";
    [Tooltip("Optional - plays a camera cut onto the boss while the wall fades. If left empty (or has no Timeline asset assigned yet), the attack-suppression window falls back to fallbackCinematicDuration instead of waiting on it.")]
    [SerializeField] private PlayableDirector cinematicDirector;
    [SerializeField] private float fallbackCinematicDuration = 3f;

    private bool gateOpened;

    private void Awake()
    {
        Enemy.AttacksDisabled = false;
    }

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
        Enemy.AttacksDisabled = true;
        inputActions?.FindActionMap("Player")?.Disable();

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

        Enemy.AttacksDisabled = false;
        EnemySpawner.ResumeAllSpawning();
        inputActions?.FindActionMap("Player")?.Enable();

        typewriter?.Play(gateOpenLine);
    }
}
