﻿/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

AudioManager.cs

Class to manage music and sound effects through an audio manager object

*/


using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background;
    public AudioClip sfxexample;
    public AudioClip shootingSound;
    public AudioClip killedSkeletonSound;
    public AudioClip grabExperienceSound;

    public AudioClip playerHitSound;
    public AudioClip playerDeadSound;
    public static AudioManager instance;

    private void Awake()
    {
        if (instance==null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() 
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

}
