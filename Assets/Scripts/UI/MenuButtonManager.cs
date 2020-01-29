using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{
    public LocalizationManager localization;
    public GameObject StartButton;
    public GameObject RestartButton;
    public GameObject ContinueButton;
    public GameObject CreditButton;
    public GameObject QuitButton;
    public GameObject SeperateLine;
    public EventSystem eventSystem;
    // Start is called before the first frame update
    void Start()
    {
        localization = GameObject.FindGameObjectWithTag("Localization").GetComponent<LocalizationManager>();
        Time.timeScale = 1f;
        if (SaveSystem.LoadPlayer() != null)
        {
            StartButton.SetActive(false);
            RestartButton.SetActive(true);
            SeperateLine.SetActive(true);
            ContinueButton.SetActive(true);
            VII.SceneManager.instance.SetSave(true);

            eventSystem.firstSelectedGameObject = ContinueButton;
        }
    }

    public void OnClickRestart()
    {
        SaveSystem.DeleteSave();
        VII.SceneManager.instance.SetSave(false);
        VII.SceneManager.instance.ResetStartValues();
    }

    public void OnClickContinue()
    {
        VII.SceneManager.instance.LoadScene(VII.SceneType.GameScene);
    }

    public void OnClickCreditBack()
    {
        if (SaveSystem.LoadPlayer() != null)
        {
            RestartButton.SetActive(true);
            SeperateLine.SetActive(true);
            ContinueButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(true);
        }
    }

    private void Update()
    {
        //Debug.Log(eventSystem.currentSelectedGameObject);
        //Debug.Log(Input.GetAxis("Horizontal"));
        if (eventSystem.currentSelectedGameObject == null && Input.GetAxis("Horizontal") != 0)
        {
            if(RestartButton.activeSelf)
                eventSystem.SetSelectedGameObject(RestartButton);
            else if(StartButton.activeSelf)
                eventSystem.SetSelectedGameObject(StartButton);
        }
        else if((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            && eventSystem.currentSelectedGameObject != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }
        //Localization
        if (localization)
        {
            if ((int)localization.Language == 0)
            {
                StartButton.GetComponentInChildren<Text>().text = "START";
                RestartButton.GetComponentInChildren<Text>().text = "RESTART";
                CreditButton.GetComponentInChildren<Text>().text = "CREDIT";
                ContinueButton.GetComponentInChildren<Text>().text = "CONTINUE";
                QuitButton.GetComponentInChildren<Text>().text = "QUIT";
            }
            else if ((int)localization.Language == 1)
            {
                StartButton.GetComponentInChildren<Text>().text = "开始游戏";
                RestartButton.GetComponentInChildren<Text>().text = "重新开始";
                CreditButton.GetComponentInChildren<Text>().text = "制作团队";
                ContinueButton.GetComponentInChildren<Text>().text = "继续游戏";
                QuitButton.GetComponentInChildren<Text>().text = "退出游戏";
            }
        }
        else
        {
            localization = GameObject.Find("LocalizationManager").GetComponent<LocalizationManager>();
        }
        
        
    }
}
