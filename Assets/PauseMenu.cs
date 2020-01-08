using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VII;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool menuDisplayed;
    void Start()
    {
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
