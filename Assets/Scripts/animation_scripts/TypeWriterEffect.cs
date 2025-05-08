using System.Collections;
using UnityEngine;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    public float typingDelay = 0.07f;
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
            Debug.LogError("Failed to load typeSound!");
        }

        audioManager = FindObjectOfType<AudioManager>();
    }


    void OnEnable()
    {
        StartCoroutine(DelayedTypewriter());
    }

    IEnumerator DelayedTypewriter()
    {
        yield return null; // waits one frame
        ShowText(textComponent.text);
    }

    public void ShowText(string fullText)
    {
        if (currentTyping != null)
            StopCoroutine(currentTyping);

        currentTyping = StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string text)
    {
        textComponent.text = "";
        foreach (char c in text)
        {
            textComponent.text += c;
            audioManager.PlaySound(typeSound);
            yield return new WaitForSeconds(typingDelay);
        }
    }
}
