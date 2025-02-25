﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// When the animation for spawning is complete
// the animation event will call the function
// by the same name on the object 
// we can then re-route this by using a unity event 
// to the monster.cs 

public class SpawnCompleteAnimationEvent : MonoBehaviour
{
    [SerializeField] UnityEvent SpawnCompleteAnimationEventCallback;

    public void SpawnComplete()
    {
        SpawnCompleteAnimationEventCallback?.Invoke();
    }
}
