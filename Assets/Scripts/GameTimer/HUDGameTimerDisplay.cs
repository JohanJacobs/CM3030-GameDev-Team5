/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

Please view README file for detailed information

*/
﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDGameTimerDisplay : MonoBehaviour
{
    [SerializeField] private Text currentTimerText;
    private Animator _currentTimerTextAnimator;

    public void Awake()
    {
        _currentTimerTextAnimator = GetComponentInChildren<Animator>();        
    }

    private void OnEnable()
    {
        GameTimer.OnTimeChangedEvent += GameTimer_OnTimeChangedEvent;        
    }

    private void OnDisable()
    {
        GameTimer.OnTimeChangedEvent -= GameTimer_OnTimeChangedEvent;
    }
    private void GameTimer_OnTimeChangedEvent(object sender, int timeLeftInSeconds)
    {
        UpdateTextDisplay(timeLeftInSeconds);
        TriggerAnimations(timeLeftInSeconds);
    }

    private void UpdateTextDisplay(int timeLeftInSeconds)
    {
        var timeSpan = TimeSpan.FromSeconds(timeLeftInSeconds); //https://learn.microsoft.com/en-us/dotnet/api/system.timespan.tostring?view=net-9.0&redirectedfrom=MSDN#System_TimeSpan_ToString_System_String_
        currentTimerText.text = timeSpan.ToString(@"mm\:ss");
    }

    private void TriggerAnimations(int timeLeftInSeconds)
    {
        if (timeLeftInSeconds < 30)
            _currentTimerTextAnimator.SetTrigger("PulseRed");
        else if (timeLeftInSeconds < 60)
            _currentTimerTextAnimator.SetTrigger("PulseWhite");
    }
}
