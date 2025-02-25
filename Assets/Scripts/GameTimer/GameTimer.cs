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
    bool _pauseTimer;

    HUD _hud;
    Action _timerRanOutCallbackFN;
    public GameTimer(float sessionLengthInSeconds, HUD hud, Action gameEndCallbackFN)
    {
        _currentTimer = sessionLengthInSeconds;
        _hud = hud;
        _timerRanOutCallbackFN = gameEndCallbackFN;
        _pauseTimer = false;
    }

    public void Update(float deltaTime)
    {
        if (_pauseTimer)        
            return;        

        // update timer values
        _currentTimer -= deltaTime;
        if (_currentTimer <= _maxTimer)
        {
            _timerRanOutCallbackFN();
            _pauseTimer = true;
        }

        // update the HUD
        _hud.SetTimerValue(_currentTimer);
    }
}
