/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

MenuButtonSpriteToggle.cs

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static VolumeSettings;

/* 
 * simple class to toggle between two sprites when a toggle method is called
 * the main use case is for the Mute button to switch between the mute and not mute 
 */
public class MuteButtonSpriteToggle: MonoBehaviour
{
    [SerializeField] Sprite notMuteSprite;
    [SerializeField] Sprite muteSprite;
    [SerializeField] VolumeSettings volumeSettings;
    [SerializeField] Image image;    
        
    private void OnEnable()
    {
        volumeSettings.OnVolumeSettingsChanged += VolumeSettings_OnVolumeSettingsChanged_Delegate;
    }
    private void OnDisable()
    {
        volumeSettings.OnVolumeSettingsChanged -= VolumeSettings_OnVolumeSettingsChanged_Delegate;
    }
        
    private void VolumeSettings_OnVolumeSettingsChanged_Delegate(VolumeSettings volumeSettings)
    {
        image.sprite = volumeSettings.IsAudioMuted ? muteSprite : notMuteSprite;
    }

}
