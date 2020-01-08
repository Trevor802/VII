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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
        if(musicSlider && soundSlider)
        {
            ins_audioManager.UpdateMusicVolume(musicSlider);
            ins_audioManager.UpdateSoundVolume(soundSlider);
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
        SceneManager.instance.LoadScene(SceneType.MainScene);
    }
}
