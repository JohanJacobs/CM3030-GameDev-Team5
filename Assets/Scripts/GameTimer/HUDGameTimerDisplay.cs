using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudTimerDisplay : MonoBehaviour
{
    [SerializeField] private Text currentTimerText;
    private Animator _currentTimerTextAnimator;

    public void Awake()
    {
        _currentTimerTextAnimator = GetComponentInChildren<Animator>();
        _currentTimerTextAnimator.enabled = false;
    }
    public void SetTimerValue(float timeLeftInSeconds)
    {
        if (currentTimerText == null)
            return;

        var timeSpan = TimeSpan.FromSeconds(timeLeftInSeconds); //https://learn.microsoft.com/en-us/dotnet/api/system.timespan.tostring?view=net-9.0&redirectedfrom=MSDN#System_TimeSpan_ToString_System_String_
        currentTimerText.text = timeSpan.ToString(@"mm\:ss");

        if (timeLeftInSeconds < 60)
        {

        }

        if (timeLeftInSeconds <= 30f)
        {
            currentTimerText.color = new Color(0.7f, 0.1f, 0.2f);
            currentTimerText.fontStyle = FontStyle.Bold;
        }
    }
    // enables or disables the animation for the scaling of the timer UI
    public void SetTimerPulseAnimation(bool state)
    {
        _currentTimerTextAnimator.enabled = state;
    }
}
