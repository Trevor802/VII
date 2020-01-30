using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VII
{
    public enum Language
    {
        EN = 0,
        CN = 1
    }
}



public class LocalizationManager : MonoBehaviour
{
    #region Singleton
    public static LocalizationManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
        if (SaveSystem.LoadPlayer() != null)
        {
            SavePlayerData data = SaveSystem.LoadPlayer();
            Instance.SwitchLanguage((VII.Language)data.saveLocal);
        }
    }
    #endregion

    public void SwitchLanguage(VII.Language i_language)
    {
        m_language = i_language;
        VII.VIIEvents.LanguageSwitch.Invoke(i_language);
    }

    private VII.Language m_language;


    // getters
    public VII.Language Language { get { return m_language; } }
}
