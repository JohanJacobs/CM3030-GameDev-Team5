using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;

public class HUD : MonoBehaviour
{
    public Slider HealthSlider;
    public Text HealthText;
    public Text KillCountText;
    public Text ExperienceText;
    public Text LevelText;
    public Text WastedText;
    public Text MessageField;
    public RectTransform InitialPanel;

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

    public void ShowWasted()
    {
        WastedText.enabled = true;
    }

    public void MsgFadeIn()
    {
        MessageField.text = "Use [W] [A] [S] [D] keys for movement \nShoot skeletons for points \nAvoid skeleton contact to remain alive \nUse mouse to aim weapon, click to fire";
        MessageField.enabled = true;
        Time.timeScale = 0;
    }

    public void MsgFadeOut()
    {
        MessageField.enabled = false;
        InitialPanel.localScale = new Vector3 (0, 0, 0);
    }

    public void gameStart()
    {
        MessageField.enabled = false;
        InitialPanel.localScale = new Vector3 (0, 0, 0);
        Time.timeScale = 1;
    }

    public void Start()
    {
        MessageField.enabled = false;
        MsgFadeIn();
    }
}
