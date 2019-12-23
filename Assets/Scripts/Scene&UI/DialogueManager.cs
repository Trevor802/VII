using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VII;

public class DialogueManager : MonoBehaviour
{
    public Text textBox;
    //public Animator animator;
    public List<string> sentences;
    public List<string> sentences_transition_trap;
    public List<string> sentences_transition_ice;
    public List<string> sentences_transition_lava;
    public float CharPopupDuration = 0.02f;
    public SceneType sceneToLoadAfterDialogue;
    private int sentenceIndex = 0;
    private bool inputAvail = false;
    public Player player;
    public Canvas transitionTextsCanvas;
    public bool displayingTexts;

    public void StartSentence()
    {
        //animator.SetBool("IsOpen", true);
        textBox.gameObject.SetActive(true);
        NextSentence();
        inputAvail = true;
    }

    void Update()
    {
        if (Input.GetKeyDown("space") && inputAvail)
        {
            NextSentence();
        }
    }

    public void NextSentence()
    {
        if (sentenceIndex >= sentences.Count)
        {
            EndSentence();
            return;
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
                if (player.display_text_trap == true)
                {
                    j = sentences_transition_trap[sentenceIndex];
                    displayingTexts = true;
                }
                else if (player.display_text_ice == true)
                {
                    j = sentences_transition_ice[sentenceIndex];
                    displayingTexts = true;
                }
                else if (player.display_text_lava == true)
                {
                    j = sentences_transition_lava[sentenceIndex];
                    displayingTexts = true;
                }
            }
        }

        sentenceIndex++;
        StartCoroutine(DisplaySentence(j));

    }

    IEnumerator DisplaySentence(string sentence)
    {
        string rt = "";
        bool caching = false;
        bool metHead = false;
        foreach(char letter in sentence.ToCharArray())
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
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0) //only loads next scene when in menu
        {
            VII.SceneManager.instance.LoadScene(sceneToLoadAfterDialogue);
        }
        else
        {
            if (transitionTextsCanvas)
            {
                transitionTextsCanvas.enabled = false;
                displayingTexts = false;
            }
        }

        inputAvail = false;
    }
}
