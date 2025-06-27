using System.Collections;
using UnityEngine;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    public float typingDelay = 0.07f; // delay between each character
    private Coroutine currentTyping;
    private TextMeshProUGUI textComponent;
    private AudioClip typeSound;
    private AudioManager audioManager;

    
    


    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        typeSound = Resources.Load<AudioClip>("soundEffects/btn_sounds/typeClick");

        
        if (typeSound == null)
        {
            Debug.LogError("failed to load typeSound");
        }

        audioManager = FindObjectOfType<AudioManager>();
    }


    void OnEnable()
    {
        //print("TypeWriterEffect enabled");

        textComponent = GetComponent<TextMeshProUGUI>();

        string fulltext = textComponent.text;// stores the full texts

        Debug.Log("Full text: " + fulltext);

        StartCoroutine(DelayedTypewriter(fulltext));
        
        textComponent.text = ""; 
    }

    IEnumerator DelayedTypewriter(string fulltext)
    {

        

        yield return null; // waits one frame
        
        ShowText(fulltext);
        
        
    }

    public void ShowText(string fullText)
    {
        if (currentTyping != null)
            StopCoroutine(currentTyping);

        currentTyping = StartCoroutine(TypeText(fullText));
    }



    private IEnumerator TypeText(string text)
    {
        
        foreach (char c in text)
        {
            textComponent.text += c;
            if (audioManager != null) { audioManager.PlaySound(typeSound); }
            yield return new WaitForSeconds(typingDelay);
        }
    }
}
