using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider FXSlider;

    private static float currentFX= 1f;
    private static float prevFX= 1f;

    private static float currentMusic = 1f;
    private static float prevMusic = 1f;
        
    private static bool isMuted = false;
    private static bool initialized = false;


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
        isMuted = !isMuted;
        Debug.Log("NO IMPLEMENTED TOGGLE MUTE INCON");
    }

    private void Start()
    {
        if (initialized)
        {
            FXSlider.value = currentFX;
            MusicSlider.value = currentMusic;
        }

        SetMixerVolume(currentFX,currentMusic);        
        initialized = true;
    }

    private void SetMixerVolume(float fxVolume, float musicVolume)
    {
        
        myMixer.SetFloat("Music", CalculateVolume(musicVolume));
        myMixer.SetFloat("SFX", CalculateVolume(fxVolume));
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

