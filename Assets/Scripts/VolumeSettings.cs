using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider FXSlider;

    float fxVolume = 1f;
    float musicVolume = 1f;
    bool isMuted = false;

    private void Start()
    {
        SetMusicVolume();
        SetFXVolume();
    }
    public void SetMusicVolume()
    {
        isMuted = false;
        musicVolume = MusicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(musicVolume) *20f);
    }

    public void SetFXVolume()
    {
        isMuted = false;
        fxVolume = FXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(fxVolume) *20f);
    }
        
    public void SetAllVolume(bool isMuted)
    {
        // verbose for readability, calculate the volume 
        var fv = (!isMuted) ? Mathf.Log10(FXSlider.value) * 20f : -80;
        var mv = (!isMuted) ? Mathf.Log10(MusicSlider.value) * 20f : -80f;

        // set the mixers
        myMixer.SetFloat("SFX", fv);
        myMixer.SetFloat("Music", mv);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        SetAllVolume(isMuted);
    }
}

