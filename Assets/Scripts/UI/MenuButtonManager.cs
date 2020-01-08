using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonManager : MonoBehaviour
{
    public GameObject StartButton;
    public GameObject RestartButton;
    public GameObject ContinueButton;
    public GameObject SeperateLine;
    // Start is called before the first frame update
    void Start()
    {
        if (SaveSystem.LoadPlayer() != null)
        {
            StartButton.SetActive(false);
            RestartButton.SetActive(true);
            SeperateLine.SetActive(true);
            ContinueButton.SetActive(true);
            VII.SceneManager.instance.SetSave(true);
        }
    }

    public void OnClickRestart()
    {
        SaveSystem.DeleteSave();
        VII.SceneManager.instance.SetSave(false);
        VII.SceneManager.instance.ResetStartIDs();
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
}
