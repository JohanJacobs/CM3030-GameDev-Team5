using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider FXSlider;

    private void Start()
    {
        SetMusicVolume();
        SetFXVolume();
    }
    public void SetMusicVolume()
    {
        float volume = MusicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume)*20);
    }

    public void SetFXVolume()
    {
        float volume = FXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
    }
}

