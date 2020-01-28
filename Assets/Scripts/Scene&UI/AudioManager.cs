using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
    }
    #endregion

    public AudioSource musicSource;
    public AudioSource soundSource;

    [Header("Audio Clips")]
    public AudioClip footStep;
    public AudioClip death;
    public AudioClip respawn;
    public AudioClip lavaSpread;
    public AudioClip slide;
    public AudioClip triggerBoard;
    public AudioClip checkpoint;
    public AudioClip collect;
    public AudioClip spikeDeath;
    public AudioClip trapDeath;

    // Play sound one time
    public void PlaySingle(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        soundSource.clip = clip;
        soundSource.Play();
    }

    // Change the background music
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void UpdateMusicVolume(Slider i_musicSlider)
    {
        musicSource.volume = i_musicSlider.value;
        VII.SceneManager.instance.SetStartMusicVolume(musicSource.volume);
    }
    public void UpdateSoundVolume(Slider i_soundSlider)
    {
        soundSource.volume = i_soundSlider.value;
        VII.SceneManager.instance.SetStartSoundVolume(soundSource.volume);
    }

    public float GetMusicVolume() { return musicSource.volume; }
    public float GetSoundVolume() { return soundSource.volume; }
}
