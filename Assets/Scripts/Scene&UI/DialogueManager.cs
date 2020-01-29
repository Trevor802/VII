using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VII;

public class DialogueManager : MonoBehaviour
{
    public Text textBox;
    public LocalizationManager localization;
    //public Animator animator;
    public List<string> sentences;
    public List<string> sentences_cn;
    public List<string> sentences_transition_trap;
    public List<string> sentences_transition_trap_cn;
    //public List<string> sentences_transition_ice;
    public List<string> sentences_transition_lava;
    public List<string> sentences_transition_lava_cn;
    public float CharPopupDuration = 0.02f;
    public SceneType sceneToLoadAfterDialogue;
    private int sentenceIndex = 0;
    private bool inputAvail = false;
    public Player player;
    public Canvas transitionTextsCanvas;
    public Animator textAnimator;
    public Animator continueAnimator;
    public Text continueText;
    public bool displayingTexts;

    private bool display_text_trap;
    //private bool display_text_ice;
    private bool display_text_lava;

    public void StartSentence()
    {
        //animator.SetBool("IsOpen", true);
        if (transitionTextsCanvas)
        {
            transitionTextsCanvas.enabled = true;
            textAnimator.SetBool("Active", true);
            continueAnimator.SetBool("Active", true);
        }
        textBox.gameObject.SetActive(true);
        NextSentence();
        inputAvail = true;
        displayingTexts = true;
    }

    void Start()
    {
        localization = GameObject.Find("LocalizationManager").GetComponent<LocalizationManager>();
    }

    void Update()
    {
        if ((Input.GetKeyDown("space") || Input.GetButtonDown("Submit")) && inputAvail)
        {
            NextSentence();
        }

        if((int)localization.Language == 0 && continueText)
        {
            continueText.text = "Press Space to Continue...";
        }
        else if((int)localization.Language == 1 && continueText)
        {
            continueText.text = "按下空格继续...";
        }
    }

    public void NextSentence()
    {
        if((int)localization.Language == 0)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
            {
                if (sentenceIndex >= sentences.Count)
                {
                    EndSentence();
                    return;
                }
            }
            else
            {
                if (transitionTextsCanvas)
                {
                    if (display_text_trap == true)
                    {
                        if (sentenceIndex >= sentences_transition_trap.Count)
                        {
                            EndSentence();
                            return;
                        }
                    }
                    /*
                    else if (display_text_ice == true)
                    {
                        if (sentenceIndex >= sentences_transition_ice.Count)
                        {
                            EndSentence();
                            return;
                        }
                    }
                    */
                    else if (display_text_lava == true)
                    {
                        if (sentenceIndex >= sentences_transition_lava.Count)
                        {
                            EndSentence();
                            return;
                        }
                    }
                }
            }
            StopAllCoroutines();
            textBox.text = "";
            // Uncomment the code block to display all sentences in one page
            /*
            for (int i = 0; i < sentenceIndex; i++)
            {
                textBox.text += sentences[i];
            }
            */
            string j = "";
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
            {
                j = sentences[sentenceIndex];
            }
            else
            {
                if (transitionTextsCanvas)
                {
                    if (display_text_trap == true)
                    {
                        j = sentences_transition_trap[sentenceIndex];
                    }
                    /*
                    else if (display_text_ice == true)
                    {
                        j = sentences_transition_ice[sentenceIndex];
                    }
                    */
                    else if (display_text_lava == true)
                    {
                        j = sentences_transition_lava[sentenceIndex];
                    }
                }
            }

            sentenceIndex++;
            StartCoroutine(DisplaySentence(j));
        }
        else if((int)localization.Language == 1)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
            {
                if (sentenceIndex >= sentences_cn.Count)
                {
                    EndSentence();
                    return;
                }
            }
            else
            {
                if (transitionTextsCanvas)
                {
                    if (display_text_trap == true)
                    {
                        if (sentenceIndex >= sentences_transition_trap_cn.Count)
                        {
                            EndSentence();
                            return;
                        }
                    }
                    /*
                    else if (display_text_ice == true)
                    {
                        if (sentenceIndex >= sentences_transition_ice.Count)
                        {
                            EndSentence();
                            return;
                        }
                    }
                    */
                    else if (display_text_lava == true)
                    {
                        if (sentenceIndex >= sentences_transition_lava_cn.Count)
                        {
                            EndSentence();
                            return;
                        }
                    }
                }
            }
            StopAllCoroutines();
            textBox.text = "";
            // Uncomment the code block to display all sentences in one page
            /*
            for (int i = 0; i < sentenceIndex; i++)
            {
                textBox.text += sentences[i];
            }
            */
            string j = "";
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
            {
                j = sentences_cn[sentenceIndex];
            }
            else
            {
                if (transitionTextsCanvas)
                {
                    if (display_text_trap == true)
                    {
                        j = sentences_transition_trap_cn[sentenceIndex];
                    }
                    /*
                    else if (display_text_ice == true)
                    {
                        j = sentences_transition_ice[sentenceIndex];
                    }
                    */
                    else if (display_text_lava == true)
                    {
                        j = sentences_transition_lava_cn[sentenceIndex];
                    }
                }
            }

            sentenceIndex++;
            StartCoroutine(DisplaySentence(j));
        }

    }

    IEnumerator DisplaySentence(string sentence)
    {
        string rt = "";
        bool caching = false;
        bool metHead = false;
        foreach (char letter in sentence.ToCharArray())
        {
            if (letter == '<')
            {
                caching = true;
            }
            else if (letter == '>')
            {
                if (metHead)
                {
                    caching = false;
                    metHead = false;
                    textBox.text += rt;
                    rt = "";
                }
                else
                {
                    metHead = true;
                }
            }
            if (!caching)
            {
                textBox.text += letter;
                yield return new WaitForSeconds(CharPopupDuration);
            }
            else
            {
                rt += letter;
            }
        }
    }

    void EndSentence()
    {
        //animator.SetBool("IsOpen", false);
        sentenceIndex = 0;
        inputAvail = false;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0) //only loads next scene when in menu
        {
            VII.SceneManager.instance.LoadScene(sceneToLoadAfterDialogue);
        }
        else
        {
            if (transitionTextsCanvas)
            {
                textAnimator.SetBool("Active", false);
                continueAnimator.SetBool("Active", false);
                displayingTexts = false;
                display_text_trap = false;
                //display_text_ice = false;
                display_text_lava = false;
                //transitionTextsCanvas.enabled = false;
            }
        }
    }
    
    public void EnableTrapText()
    {
        display_text_trap = true;
    }
    /*
    public void EnableIceText()
    {
        display_text_ice = true;
    }
    */
    public void EnableLavaText()
    {
        display_text_lava = true;
    }
}
