using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;
using System;
using System.Collections;


public class HUD : MonoBehaviour
{
    public Slider HealthSlider;
    public Text HealthText;
    public Text KillCountText;
    public Text ExperienceText;
    public Text LevelText;
    public GameObject GameOver;

    [SerializeField] private Text highestScoreText;
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text newHighScoreText;
    [SerializeField] private Text currentTimerText;
           
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

    public void ShowGameOver(int highestScore, int currentScore)
    {
        UpdateScoreText(highestScore,currentScore);
        GameOver.SetActive(true);
    }

    private void UpdateScoreText(int highestScore, int currentScore)
    {
        // Update the text displayed in the UI
        highestScoreText.text = $"BEST:\n {highestScore.ToString(CultureInfo.InvariantCulture)}";
        currentScoreText.text = $"CURRENT:\n {currentScore.ToString(CultureInfo.InvariantCulture)}";

        // Update text color to green if the new score is higher, else keep the same color
        if (currentScore > highestScore)
        {
            currentScoreText.color = new Color(.15f, .70f, .2f);
            currentScoreText.fontStyle = FontStyle.Bold;
            newHighScoreText.color = new Color(.15f, .70f, .2f);
        }
        else
        {
            currentScoreText.color = new Color(1f, 1f, 1f);
            currentScoreText.fontStyle = FontStyle.Normal;
            newHighScoreText.enabled = false;
        }
    }
    public void OnRestartClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    #region TimerDisplay
    public void SetTimerValue(float timeLeftInSeconds)
    {
        if (currentTimerText == null)
            return;

        var timeSpan = TimeSpan.FromSeconds(timeLeftInSeconds); //https://learn.microsoft.com/en-us/dotnet/api/system.timespan.tostring?view=net-9.0&redirectedfrom=MSDN#System_TimeSpan_ToString_System_String_
        currentTimerText.text = timeSpan.ToString(@"mm\:ss");

        if (timeLeftInSeconds < 60)
        {
            StartCoroutine(PulseGameTimerSize());
        }

        if (timeLeftInSeconds <= 30f)
        {
            currentTimerText.color = new Color(0.7f, 0.1f, 0.2f);
            currentTimerText.fontStyle = FontStyle.Bold;
        }
    }
    IEnumerator PulseGameTimerSize()
    {
        var scale = 0.9f;
        while(scale < 1f)
        {
            currentTimerText.rectTransform.localScale = new Vector3(scale, scale, scale);
            yield return null;
            scale += 0.01f;  // decrease by 90%
        }
        currentTimerText.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    #endregion TimerDisplay
}
