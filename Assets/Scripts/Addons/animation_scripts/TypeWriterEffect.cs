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


    private IEnumerator TypeText()
    {
        
        
        foreach (char c in currentText)
        {
            textComponent.text += c;
            if (audioManager != null) { audioManager.PlaySoundWithPich(typeSound, pitch); }
            yield return new WaitForSeconds(typingDelay);
        }

        yield return new WaitForSeconds(timeBetweenLines);

        hasFinishedTyping = true; 

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
