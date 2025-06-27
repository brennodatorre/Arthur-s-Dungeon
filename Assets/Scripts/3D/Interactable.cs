using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    Outline outline;

    public string message;

    public UnityEvent onInteract; // Event to trigger when interacted with

    

    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void disableOutline()
    {
        outline.enabled = false;
    }

    public void enableOutline()
    {
        outline.enabled = true;
    }

    public void interact()
    {
        // Trigger the interaction event
        onInteract.Invoke();
    }

  
}
