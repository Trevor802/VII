using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VII;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public Slider musicSlider;
    public Slider soundSlider;
    private AudioManager ins_audioManager;
    private bool menuDisplayed;

    void Start()
    {
        ins_audioManager = AudioManager.instance;
        pauseMenu.SetActive(false);
        menuDisplayed = false;
        if (SceneManager.instance.GetSave())
        {
            SavePlayerData data = SaveSystem.LoadPlayer();
            musicSlider.value = data.saveMusicVolume;
            soundSlider.value = data.saveSoundVolume;
        }
        else
        {
            musicSlider.value = SceneManager.instance.GetStartMusicVolume();
            soundSlider.value = SceneManager.instance.GetStartSoundVolume();
            UpdateMusicVolume();
            UpdateSoundVolume();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Menu"))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if(menuDisplayed)
        {
            pauseMenu.SetActive(false);
            menuDisplayed = false;
            Time.timeScale = 1f;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            menuDisplayed = true;
        }
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.instance.LoadScene(SceneType.MainScene);
    }

    public void UpdateMusicVolume()
    {
        ins_audioManager.UpdateMusicVolume(musicSlider);
    }

    public void UpdateSoundVolume()
    {
        ins_audioManager.UpdateSoundVolume(soundSlider);
    }
}
