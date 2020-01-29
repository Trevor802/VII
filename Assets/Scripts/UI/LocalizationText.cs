using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizationText : MonoBehaviour
{
    public string[] contents;

    private void Awake()
    {
        VII.VIIEvents.LanguageSwitch.AddListener(UpdateUI);
    }

    private void OnEnable()
    {
        GetComponent<Text>().text = contents[(int)LocalizationManager.Instance.Language];
    }

    private void UpdateUI(VII.Language i_language)
    {
        GetComponent<Text>().text = contents[(int)i_language];
    }
}
