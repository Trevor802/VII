using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    public Image ButtonImage;
    public Text ButtonText;
    public Sprite[] sprites;
    public string[] titles;
    private void Awake()
    {
        VII.VIIEvents.LanguageSwitch.AddListener(UpdateUI);
    }
    public void SwitchLanguage()
    {
        VII.Language language = LocalizationManager.Instance.Language;
        int index = (int)language;
        index++;
        language = (VII.Language)index;
        if (!Enum.IsDefined(typeof(VII.Language), language))
            language = VII.Language.EN;
        LocalizationManager.Instance.SwitchLanguage(language);
        PlayerPrefs.SetInt("saveLocal", (int)language);
    }

    private void UpdateUI(VII.Language i_language)
    {
        ButtonImage.sprite = sprites[(int)i_language];
        ButtonText.text = titles[(int)i_language];
    }
}
