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

    private static float currentFX = 1f;
    private static float prevFX = 1f;

    private static float currentMusic = 1f;
    private static float prevMusic = 1f;

    private static bool isMuted = false;
    private static bool initialized = false;


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
        if (initialized)
        {
            FXSlider.value = currentFX;
            MusicSlider.value = currentMusic;
            OnVolumeSettingsChanged?.Invoke(this);
        }

        SetMixerVolume(currentFX,currentMusic);

        initialized = true;
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

        
    //public void SetMusicVolume()
    //{
    //    isMuted = false;
    //    musicVolume = MusicSlider.value;
    //    myMixer.SetFloat("Music", Mathf.Log10(musicVolume) *20f);
    //}

    //public void SetFXVolume()
    //{
    //    isMuted = false;
    //    fxVolume = FXSlider.value;
    //    myMixer.SetFloat("SFX", Mathf.Log10(fxVolume) *20f);
    //}
        
    //public void SetAllVolume(bool isMuted)
    //{
    //    // verbose for readability, calculate the volume 
    //    var fv = (!isMuted) ? Mathf.Log10(FXSlider.value) * 20f : -80;
    //    var mv = (!isMuted) ? Mathf.Log10(MusicSlider.value) * 20f : -80f;

    //    // set the mixers
    //    myMixer.SetFloat("SFX", fv);
    //    myMixer.SetFloat("Music", mv);
    //}
        

}

