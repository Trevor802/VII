using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditMenu : MonoBehaviour
{
    public EventSystem eventSystem;
    public GameObject backButton;

    private void OnEnable()
    {
        eventSystem.SetSelectedGameObject(backButton);
    }

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject == null && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            eventSystem.SetSelectedGameObject(backButton);
        }
        else if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            && eventSystem.currentSelectedGameObject != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }
        else if(Input.GetButtonDown("Cancel"))
        {
            backButton.GetComponent<Button>().onClick.Invoke();
        }
    }
}
