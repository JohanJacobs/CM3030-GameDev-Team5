/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

VolumeSettings.cs

Class to manage volume settings for music and sound FX. Uses an Audio Mixer to independently control the volume. 

*/

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static Pickup;
using static VolumeSettings;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider FXSlider;

    private static float currentFX = 0.4f;
    private static float prevFX = 0.4f;

    private static float currentMusic = 0.2f;
    private static float prevMusic = 0.2f;

    private static bool isMuted = false;

    public delegate void VolumeSettingsChanged(VolumeSettings volumeSettings);
    public event VolumeSettingsChanged OnVolumeSettingsChanged;

    public bool IsAudioMuted { get { return isMuted; } }
    
    public void FXSliderChanged()
    {
        currentFX = FXSlider.value;
        SetMixerVolume(currentFX, currentMusic);        
    }


    public void MusicSliderChanged()
    {
        currentMusic = MusicSlider.value;
        SetMixerVolume(currentFX, currentMusic);        
    }

    public void ToggleMute()
    {
        if(isMuted==true)
        {
            //restore values the slicers had
            currentFX = prevFX;
            currentMusic = prevMusic;

            // update state
            isMuted = false;
        }
        else if (isMuted==false)
        {
            // backup the values in the slicers
            prevFX = currentFX;
            prevMusic = currentMusic;

            currentFX = 0;
            currentMusic =0;

            // update state 
            isMuted = true;
        }

        // update visuals, setting these values triggers the callback 
        // that updates the volume
        MusicSlider.value = currentFX;
        FXSlider.value = currentFX;                
    }
       
    private void Start()
    {
        {
            FXSlider.value = currentFX;
            MusicSlider.value = currentMusic;
            //OnVolumeSettingsChanged?.Invoke(this);
        }

        //SetMixerVolume(currentFX,currentMusic);

        
    }

    private void SetMixerVolume(float fxVolume, float musicVolume)
    {
        myMixer.SetFloat("Music", CalculateVolume(musicVolume));
        myMixer.SetFloat("SFX", CalculateVolume(fxVolume));
        OnVolumeSettingsChanged?.Invoke(this);
    }

    private float CalculateVolume(float sliderValue)
    {
        return Mathf.Log10(sliderValue) * 20f;
    }         
   
}

