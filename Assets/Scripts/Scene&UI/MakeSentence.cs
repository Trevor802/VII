using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSentence : MonoBehaviour
{
    private bool displayLevel0_Sentence1;
    private bool displayLevel1_Sentence1;
    private bool displayLevel1_Sentence2;
    private bool displayLevel7_Sentence1;

    private bool deactivateSentence;


    // Update is called once per frame
    void Update()
    {
        
    }

    void EnableLevel0_Sentence1()
    {
        displayLevel0_Sentence1 = true;
    }

    void EnableLevel1_Sentence1()
    {
        displayLevel1_Sentence1 = true;
    }

    void EnableLevel1_Sentence2()
    {
        displayLevel1_Sentence2 = true;
    }

    void EnableLevel7_Sentence1()
    {
        displayLevel7_Sentence1 = true;
    }

    void deactivate()
    {
        deactivateSentence = true;
    }
}
