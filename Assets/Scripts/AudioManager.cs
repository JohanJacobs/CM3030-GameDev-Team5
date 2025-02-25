/*

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
    public static AudioManager Instance
    {
        get
        {
            if (!_instance)
            {
                var prefab = Resources.Load<AudioManager>("DefaultAudioManager");

                _instance = Instantiate(prefab);

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background;
    public AudioClip sfxexample;
    public AudioClip shootingSound;
    public AudioClip killedSkeletonSound;
    public AudioClip grabExperienceSound;

    public AudioClip playerHitSound;
    public AudioClip playerDeadSound;

    public void Initialize()
    {
        // NOTE: keep it as it's used instantiate AudioManager in time
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    private static AudioManager _instance;
}