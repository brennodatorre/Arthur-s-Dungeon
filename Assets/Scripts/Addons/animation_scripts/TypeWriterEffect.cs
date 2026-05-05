using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class TypeWriterEffect : MonoBehaviour
{
    [Tooltip("Delay between each character being typed")]
    public float typingDelay = 0.07f; // delay between each character
    private Coroutine currentTyping;
    private TextMeshProUGUI textComponent;
    private AudioClip typeSound;
    private AudioManager audioManager;

    [Tooltip("Delay between lines of text when using StartNewTyping")]
    public float timeBetweenLines = 1f; // delay between lines of text

    public string currentText; 
    // list to store all typed texts that need to be typed out
    public List<string> typingQueue = new List<string>(); 

    public bool hasFinishedTyping = false; 
    public float pitch;

    [Tooltip("Flag to indicate if the player wants to skip the typing effect")]
    public bool wantsToSkip = false; 

    //before starts
    private void Awake()
    {
       
        textComponent = GetComponent<TextMeshProUGUI>();

        typeSound = Resources.Load<AudioClip>("soundEffects/btn_sounds/typeClick");

        if (typeSound == null) { Debug.LogError("failed to load typeSound"); }

        audioManager = FindObjectOfType<AudioManager>();


        //Debug.Log("Full text: " + fullText);

        currentText = textComponent.text;
    }

    void OnEnable()
    {
        //print("TypeWriterEffect enabled");

        currentText = textComponent.text;

        textComponent.text = ""; // clear the text component before typing

        currentTyping = StartCoroutine(TypeText());
        
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasFinishedTyping)
        {
            wantsToSkip = true; // set the flag to indicate the player wants to skip
        }
    } 


    private IEnumerator TypeText()
    {
        
        
        foreach (char c in currentText)
        {
            textComponent.text += c;
            if (audioManager != null && !wantsToSkip) { audioManager.PlaySoundWithPich(typeSound, pitch); }
            if (wantsToSkip) yield return new WaitForSeconds(0.01f); // if the player wants to skip, don't wait between characters
            else yield return new WaitForSeconds(typingDelay); // wait for the specified delay before typing the next character
            
        }

        

        yield return new WaitForSeconds( timeBetweenLines);

        hasFinishedTyping = true; 
        wantsToSkip = false; // reset the skip flag for the next time we type

        if (typingQueue.Count > 0)
        {
            string nextText = typingQueue[0]; // get the next text from the queue
            typingQueue.RemoveAt(0); // remove it from the queue
            TypeNext(nextText); // start typing the next text
        }

    }


    public void TypeNext(string newText)
    {


        if (currentTyping != null && !hasFinishedTyping)
        {
            typingQueue.Add(newText); // add the new text to the queue if we're still typing
            return; // exit the method to wait for current typing to finish
        }

        currentText = newText;
        textComponent.text = ""; // clear the text component before typing
        hasFinishedTyping = false; // reset the flag for new typing
        currentTyping = StartCoroutine(TypeText());
    }
}
