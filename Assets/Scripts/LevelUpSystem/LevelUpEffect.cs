using System.Collections;
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
        LevelUpSystem.GlobalLevelUpEvent += LevelUpSysotem_GlobalLevelUpEvent;
    }
    private void OnDisable()
    {
        LevelUpSystem.GlobalLevelUpEvent -= LevelUpSysotem_GlobalLevelUpEvent;
    }

    private void LevelUpSysotem_GlobalLevelUpEvent(int LevelNumber)
    {
        _popupManager?.CreateNewPopup(transform.position, $"Level {LevelNumber++}!",transform,3f);
        particleExplode.Play();
        particleSwirl.Play();
    }
}
