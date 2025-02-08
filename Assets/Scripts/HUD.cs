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
    public int MsgNumber;

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
        switch(MsgNumber)
        {
            case 0: 
                MessageField.text = "Use [W] [A] [S] [D] keys for movement";
                break;
            case 1: 
                MessageField.text = "Shoot skeletons for points";
                break;
            case 2: 
                MessageField.text = "Avoid skeleton contact to remain alive";
                break;
            case 3: 
                MessageField.text = "Use mouse to aim weapon, click to fire";
                break;
        }

        MsgNumber++;
        MessageField.enabled = true;
    }

    public void MsgFadeOut()
    {
        MessageField.enabled = false;
    }

    public void Start()
    {
        MsgNumber = 0;
        MessageField.enabled = false;
        Invoke("MsgFadeIn", 2.0f);
        Invoke("MsgFadeOut", 4.0f);
        Invoke("MsgFadeIn", 6.0f);
        Invoke("MsgFadeOut", 8.0f);
        Invoke("MsgFadeIn", 10.0f);
        Invoke("MsgFadeOut", 12.0f);
        Invoke("MsgFadeIn", 14.0f);
        Invoke("MsgFadeOut", 16.0f);
    }
}
