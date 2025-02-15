using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;


public class HUD : MonoBehaviour
{
    public Slider HealthSlider;
    public Text HealthText;
    public Text KillCountText;
    public Text ExperienceText;
    public Text LevelText;
    public GameObject GameOver;

    public void UpdateHealth(float health, float maxHealth)
    {
        HealthSlider.value = health / maxHealth;
        HealthText.text = health.ToString("0", CultureInfo.InvariantCulture);
    }

    public void UpdateKillCounter(int killCount)
    {
        KillCountText.text = killCount.ToString(CultureInfo.InvariantCulture);
    }

    public void UpdateExperience(float experience)
    {
        ExperienceText.text = Mathf.CeilToInt(experience).ToString(CultureInfo.InvariantCulture);
    }

    public void UpdateLevel(float level)
    {
        LevelText.text = Mathf.FloorToInt(level).ToString(CultureInfo.InvariantCulture);
    }

    public void ShowGameOver()
    {
        GameOver.SetActive(true);
    }

    public void OnRestartClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
}
