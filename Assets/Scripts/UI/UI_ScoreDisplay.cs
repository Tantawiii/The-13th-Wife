using TMPro;
using UnityEngine;

public class UI_ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        if (text == null || ScoreManager.Instance == null)
            return;

        int minutes = Mathf.FloorToInt(ScoreManager.Instance.SurvivalTime / 60f);
        int seconds = Mathf.FloorToInt(ScoreManager.Instance.SurvivalTime % 60f);
        text.text = $"Enemies Defeated: {ScoreManager.Instance.EnemiesDefeated}\nTime: {minutes:00}:{seconds:00}";
    }
}
