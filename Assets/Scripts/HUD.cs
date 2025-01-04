using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Slider HealthSlider;
    public Text HealthText;
    public Text KillCountText;
    public Text WastedText;

    public void UpdateHealth(float health, float maxHealth)
    {
        HealthSlider.value = health / maxHealth;
        HealthText.text = health.ToString("0", CultureInfo.InvariantCulture);
    }

    public void UpdateKillCounter(int killCount)
    {
        KillCountText.text = killCount.ToString(CultureInfo.InvariantCulture);
    }

    public void ShowWasted()
    {
        WastedText.enabled = true;
    }
}
