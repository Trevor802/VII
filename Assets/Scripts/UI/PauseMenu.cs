using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VII;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public Slider musicSlider;
    public Slider soundSlider;
    public GameObject layerToHide;
    public EventSystem eventSystem;
    public GameObject defaultButton;
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
        else if(Input.GetButtonDown("MainMenu"))
        {
            ToMainMenu();
        }
        else if(Input.GetButtonDown("Cancel") && menuDisplayed)
        {
            ToggleMenu();
        }

        if (eventSystem.currentSelectedGameObject == null && Input.GetAxis("Vertical") != 0)
        {
            if (defaultButton.activeSelf)
                eventSystem.SetSelectedGameObject(defaultButton);
        }
        else if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            && eventSystem.currentSelectedGameObject != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }

    public void ToggleMenu()
    {
        if(menuDisplayed)
        {
            layerToHide.SetActive(true);
            pauseMenu.SetActive(false);
            menuDisplayed = false;
            Time.timeScale = 1f;
        }
        else
        {
            layerToHide.SetActive(false);
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            menuDisplayed = true;
        }
    }

    public void ToMainMenu()
    {
        SceneManager.instance.LoadScene(SceneType.MainScene);
        Time.timeScale = 1f;
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
