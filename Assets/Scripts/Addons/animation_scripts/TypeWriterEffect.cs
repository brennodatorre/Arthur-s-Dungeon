using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class TypeWriterEffect : MonoBehaviour
{
    public float typingDelay = 0.07f; // delay between each character
    private Coroutine currentTyping;
    private TextMeshProUGUI textComponent;
    private AudioClip typeSound;
    private AudioManager audioManager;

    public string fullText; // stores the full text to be displayed

    //before starts
    private void Awake()
    {
       
        textComponent = GetComponent<TextMeshProUGUI>();

        typeSound = Resources.Load<AudioClip>("soundEffects/btn_sounds/typeClick");

        if (typeSound == null) { Debug.LogError("failed to load typeSound"); }

        audioManager = FindObjectOfType<AudioManager>();


        //Debug.Log("Full text: " + fullText);

        fullText = textComponent.text;
    }

    void OnEnable()
    {
        //print("TypeWriterEffect enabled");

        textComponent.text = ""; // clear the text component before typing

        StartCoroutine(TypeText());
        
    }


    private IEnumerator TypeText()
    {
        
        
        
        foreach (char c in fullText)
        {
            textComponent.text += c;
            if (audioManager != null) { audioManager.PlaySound(typeSound); }
            yield return new WaitForSeconds(typingDelay);
        }
    }
}
