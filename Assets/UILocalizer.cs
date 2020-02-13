using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILocalizer : MonoBehaviour
{
    public LocalizationManager localization;
    public GameObject RestartReminder_1;
    public GameObject RestartReminder_2;
    public GameObject BGM_text;
    public GameObject SFX_text;
    public GameObject Speed_text;
    public GameObject BackButton;
    public GameObject MainMenuButton;
    public GameObject GameSave_text;
    // Start is called before the first frame update
    void Start()
    {
        localization = GameObject.Find("LocalizationManager").GetComponent<LocalizationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(localization)
        {
            if ((int)localization.Language == 0)
            {
                RestartReminder_1.GetComponent<Text>().text = "Press";
                RestartReminder_2.GetComponent<Text>().text = "to Restart Level";
                BGM_text.GetComponent<Text>().text = "BGM";
                SFX_text.GetComponent<Text>().text = "Sound Effects";
                Speed_text.GetComponent<Text>().text = "Player Speed";
                BackButton.GetComponent<Text>().text = "Back (B)";
                MainMenuButton.GetComponent<Text>().text = "Main Menu (X)";
                GameSave_text.GetComponent<Text>().text = "Game Saved";
            }
            else if ((int)localization.Language == 1)
            {
                RestartReminder_1.GetComponent<Text>().text = "按";
                RestartReminder_2.GetComponent<Text>().text = "重启本关卡";
                BGM_text.GetComponent<Text>().text = "背景音乐";
                SFX_text.GetComponent<Text>().text = "音效";
                Speed_text.GetComponent<Text>().text = "移动速度";
                BackButton.GetComponent<Text>().text = "返回 (B)";
                MainMenuButton.GetComponent<Text>().text = "主菜单 (X)";
                GameSave_text.GetComponent<Text>().text = "游戏已保存";
            }
        }
    }
}
