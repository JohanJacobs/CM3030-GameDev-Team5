using System;
using UnityEngine;

// Game timer is a timer that runs from a value to zero. 
// when the value reaches zero, then the gameEndCallbackFN action is executed
public class GameTimer
{
    // Event that is triggered when the timer reaches zero
    public static EventHandler OnTimerRanOutEvent;

    // Event that is triggered when a second has passed;
    public static EventHandler<int> OnTimeChangedEvent; 

    float _timeLeft;

    public GameTimer(float sessionLengthInSeconds)
    {
        _timeLeft = sessionLengthInSeconds;       
    }

    public void Update()
    {   
        var oldtimeLeftInt = Mathf.FloorToInt(_timeLeft);
        _timeLeft -= Time.deltaTime;    
        var newTimeLeftInt = Mathf.FloorToInt(_timeLeft);

        // Trigger event that a second has passed, up to and including the last 0 and not negative time
        if (oldtimeLeftInt > newTimeLeftInt && newTimeLeftInt >= 0)
        {
            OnTimeChangedEvent?.Invoke(this, newTimeLeftInt);
        }

        // All the time for the timer has run out
        if (newTimeLeftInt == 0)
        {
            OnTimerRanOutEvent?.Invoke(this, new EventArgs());
        }
    }
}
