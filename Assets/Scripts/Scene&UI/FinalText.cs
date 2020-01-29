using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalText : MonoBehaviour
{

    public Text finalText1;
    public Text finalText2;
    public LocalizationManager localization;

    void Start()
    {
        localization = GameObject.Find("LocalizationManager").GetComponent<LocalizationManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (finalText1 && finalText2)
        {
            if ((int)localization.Language == 0)
            {
                finalText1.text = "What sleeps is aroused,";
                finalText2.text = "Once more.";

            }
            else if ((int)localization.Language == 1)
            {
                finalText1.text = "...您的力量已经完全恢复";
                finalText2.text = "欢迎您的莅临";
            }
        }
    }
}
