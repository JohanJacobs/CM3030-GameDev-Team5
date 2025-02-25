using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game timer is a timer that runs from a value to zero. 
// when the value reaches zero, then the gameEndCallbackFN action is executed
public class GameTimer
{
    float _maxTimer;
    float _currentTimer;
    float _cumulator;
    bool _timerStopped;

    HUD _hud;
    Action _timerRanOutCallbackFN;
    public GameTimer(float sessionLengthInSeconds, HUD hud, Action gameEndCallbackFN)
    {
        _currentTimer = sessionLengthInSeconds;
        _hud = hud;
        _timerRanOutCallbackFN = gameEndCallbackFN;
        _timerStopped = false;
        _cumulator = 0f; 
    }

    public void Update(float deltaTime)
    {
        if (_timerStopped)        
            return;

        // accumulate 1 second of time then update the timer and display
        _cumulator += deltaTime;

        if (_cumulator > 1f)
        {
            UpdateGameTime();
            UpdateHudDisplay();
            _cumulator -= 1f;
        }
    }

    private void UpdateGameTime()
    {
        _currentTimer -= _cumulator;
        if (_currentTimer <= _maxTimer)
        {
            _timerRanOutCallbackFN();
            _timerStopped = true;
        }
    }
    private void UpdateHudDisplay()
    {        
        _hud.SetTimerValue(_currentTimer);
    }
}
