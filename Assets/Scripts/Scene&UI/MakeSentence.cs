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

    private bool displayedLevel0_Sentence1;
    private bool displayedLevel1_Sentence1;
    private bool displayedLevel1_Sentence2;
    private bool displayedLevel7_Sentence1;

    private bool deactivateSentence;

    public Text text;
    public Animator animator;
    // Update is called once per frame
    void Update()
    {
        if(deactivateSentence == true)
        {
            animator.SetBool("Active", false);
            deactivateSentence = false;
        }

        if(displayLevel0_Sentence1 == true && !displayedLevel0_Sentence1)
        {
            animator.SetBool("Active", true);
            text.text = "You look much better now!";
            displayedLevel0_Sentence1 = true;
        }

        if(displayLevel1_Sentence1 == true && !displayedLevel1_Sentence1)
        {
            animator.SetBool("Active", true);
            text.text = "Be careful of their feeble flesh...";
            displayedLevel1_Sentence1 = true;
        }

        if (displayLevel1_Sentence2 == true && !displayedLevel1_Sentence2)
        {
            animator.SetBool("Active", true);
            text.text = "...Yet their sacrifice is destined";
            displayedLevel1_Sentence2 = true;
        }

        if (displayLevel7_Sentence1 == true && !displayedLevel7_Sentence1)
        {
            animator.SetBool("Active", true);
            text.text = "Ah, a nice resting place for their exhausted bodies.";
            displayedLevel7_Sentence1 = true;
        }


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

    public void deactivate()
    {
        deactivateSentence = true;
    }
}
