using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSelect : MonoBehaviour
{
    public GameObject startButton;
    public GameObject continueButton;
    private EventSystem eventSystem;

    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
        if(startButton.activeSelf)
        {
            eventSystem.firstSelectedGameObject = startButton;
        }
        else if(continueButton.activeSelf)
        {
            eventSystem.firstSelectedGameObject = continueButton;
        }
    }

    void Update()
    {
        Debug.Log(eventSystem.currentSelectedGameObject);
    }
}
