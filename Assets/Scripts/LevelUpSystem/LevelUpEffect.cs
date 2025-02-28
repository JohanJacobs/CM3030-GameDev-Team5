﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem particleExplode;
    [SerializeField] ParticleSystem particleSwirl;
    PopupManager _popupManager;

    private void Awake()
    {
        var game = GameObject.FindGameObjectWithTag("GameController");
        _popupManager = game.GetComponent<PopupManager>();
    }
    private void OnEnable()
    {
        LevelUpSystem.GlobalLevelUpEvent += LevelUpSystem_GlobalLevelUpEvent;
    }
    private void OnDisable()
    {
        LevelUpSystem.GlobalLevelUpEvent -= LevelUpSystem_GlobalLevelUpEvent;
    }

    // Player Leveled up 
    private void LevelUpSystem_GlobalLevelUpEvent(int LevelNumber)
    {
        // Display a message for the level up
        _popupManager?.CreateNewPopup(transform.position, $"Level {LevelNumber++}!",transform,3f);

        // Trigger the particle systems
        particleExplode.Play();
        particleSwirl.Play();
    }
}
