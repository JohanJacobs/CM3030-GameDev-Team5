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
    Image image;    

    private void Start()
    {
        image = GetComponent<Image>();
        VolumeSettings_OnVolumeSettingsChanged_Delegate(volumeSettings);
    }
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
        if (volumeSettings.IsAudioMuted)
        {
            image.sprite = muteSprite;
        }
        else
        {
            image.sprite = notMuteSprite;
        }
    }
}
