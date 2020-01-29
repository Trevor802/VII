using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeSentence : MonoBehaviour
{
    private bool displayLevel0_Sentence1;
    private bool displayLevel1_Sentence1;
    private bool displayLevel1_Sentence2;
    private bool displayLevel7_Sentence1;
    private bool displayFinalLevel_Sentence1;

    private bool displayedLevel0_Sentence1;
    private bool displayedLevel1_Sentence1;
    private bool displayedLevel1_Sentence2;
    private bool displayedLevel7_Sentence1;
    private bool displayedFinalLevel_Sentence1;

    private bool deactivateSentence;

    private bool m_bAutoDisable = true;

    public Text text;
    public Animator animator;
    public bool displayingText;
    public LocalizationManager localization;

    

    void Start()
    {
        localization = GameObject.Find("LocalizationManager").GetComponent<LocalizationManager>();
    }
    // Update is called once per frame
    void Update()
    {
        
        //print(deactivateSentence + " " + displayingText);
        if(deactivateSentence == true && displayingText == true && m_bAutoDisable)
        {
            animator.SetBool("Active", false);
            deactivateSentence = false;
            displayingText = false;
        }

        if(displayLevel0_Sentence1 == true && !displayedLevel0_Sentence1)
        {
            animator.SetBool("Active", true);
            if((int)localization.Language == 0)
            {
                text.text = "You look much better now!";
            }
            else if((int)localization.Language == 1)
            {
                text.text = "您看起来恢复了一些";
            }
            
            displayedLevel0_Sentence1 = true;
            displayingText = true;
        }

        if(displayLevel1_Sentence1 == true && !displayedLevel1_Sentence1)
        {
            animator.SetBool("Active", true);
            if ((int)localization.Language == 0)
            {
                text.text = "Be careful of their feeble flesh...";
            }
            else if ((int)localization.Language == 1)
            {
                text.text = "他们的身体总不是很牢靠...";
            }
            
            displayedLevel1_Sentence1 = true;
            displayingText = true;
        }

        if (displayLevel1_Sentence2 == true && !displayedLevel1_Sentence2)
        {
            animator.SetBool("Active", true);
            if ((int)localization.Language == 0)
            {
                text.text = "...Yet their sacrifice is destined";
            }
            else if ((int)localization.Language == 1)
            {
                text.text = "...但我相信您并不会介意一些牺牲";
            }
            
            displayedLevel1_Sentence2 = true;
            displayingText = true;
        }

        if (displayLevel7_Sentence1 == true && !displayedLevel7_Sentence1)
        {
            animator.SetBool("Active", true);
            if ((int)localization.Language == 0)
            {
                text.text = "Ah, a nice resting place for their exhausted bodies.";
            }
            else if ((int)localization.Language == 1)
            {
                text.text = "噢, 看来他不得不永远停留此处";
            }
            
            displayedLevel7_Sentence1 = true;
            displayingText = true;
        }

        if(displayFinalLevel_Sentence1 == true && !displayedFinalLevel_Sentence1)
        {
            animator.SetBool("Active", true);
            if ((int)localization.Language == 0)
            {
                text.text = "When the Pieces are back together.";
            }
            else if ((int)localization.Language == 1)
            {
                text.text = "一切都已准备就绪...";
            }
            
            displayedLevel7_Sentence1 = true;
            displayingText = true;
        }


    }
    public void DisplayLastSentence()
    {
        animator.SetBool("Active", true);
        if ((int)localization.Language == 0)
        {
            text.text = "What sleeps is aroused,\nOnce more.";
        }
        else if ((int)localization.Language == 1)
        {
            text.text = "...您的力量已经完全恢复\n欢迎您的莅临";
        }
        
        displayingText = true;
        m_bAutoDisable = false;
    }

    public void EnableLevel0_Sentence1()
    {
        displayLevel0_Sentence1 = true;
    }

    public void EnableLevel1_Sentence1()
    {
        displayLevel1_Sentence1 = true;
    }

    public void EnableLevel1_Sentence2()
    {
        displayLevel1_Sentence2 = true;
    }

    public void EnableLevel7_Sentence1()
    {
        displayLevel7_Sentence1 = true;
    }

    public void EnableFinalLevel_Sentence1()
    {
        displayedFinalLevel_Sentence1 = true;
    }

    public void deactivate()
    {
        deactivateSentence = true;
    }
}
