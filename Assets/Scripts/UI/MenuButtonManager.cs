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
            VII.SceneManager.instance.hasSave = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
