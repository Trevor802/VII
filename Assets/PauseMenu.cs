using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if(Input.GetKeyDown(KeyCode.Escape) && !menuDisplayed)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            menuDisplayed = true;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && menuDisplayed)
        {
            pauseMenu.SetActive(false);
            menuDisplayed = false;
            Time.timeScale = 1f;
        }
    }
}
