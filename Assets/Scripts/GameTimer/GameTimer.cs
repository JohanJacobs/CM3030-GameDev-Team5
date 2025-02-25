using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game timer is a timer that runs from a value to zero. 
// when the value reaches zero, then the gameEndCallbackFN action is executed
public class GameTimer
{
    float _sessionEndAt;
    float _sessionStartAt;

    float _nextVisualUpdateTime;
    
    bool _timerStopped;

    HUD _hud;
    Action _timerRanOutCallbackFN;
    public GameTimer(float sessionLengthInSeconds, HUD hud, Action gameEndCallbackFN)
    {
        _sessionStartAt = Time.time;
        _sessionEndAt = Time.time + sessionLengthInSeconds;
        _nextVisualUpdateTime = Time.time + 1f;

        _hud = hud;
        _timerRanOutCallbackFN = gameEndCallbackFN;

        _timerStopped = false;

        UpdateHudDisplay();
    }

    public void Update()
    {        
        if (_timerStopped)        
            return;
                
        if (Time.time >= _nextVisualUpdateTime)
        {
            UpdateGameTime();
            UpdateHudDisplay();

            _nextVisualUpdateTime += 1f; // wait for 1 second to udpate visuals
        }
    }

    private void UpdateGameTime()
    {        
        if (Time.time >=_sessionEndAt)
        {
            _timerRanOutCallbackFN();
            _timerStopped = true;
        }
    }
    private void UpdateHudDisplay()
    {

        _hud.SetTimerValue(_sessionEndAt - Time.time);
    }
}
