using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class hudController : MonoBehaviour
{

    public static hudController instance; // makes this class a singleton

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] GameObject InteractionText;

    private void Start()
    {
        InteractionText.SetActive(false);
    }

    public void EnableInteractionText(string text)
    {
        InteractionText.gameObject.SetActive(true);

        
        InteractionText.GetComponentInChildren<TextMeshProUGUI>().text = text + "(E)";

        
    }

    public void DesableInteractionText()
    {
        //InteractionText.GetComponentInChildren<TextMeshProUGUI>().text = "";

        InteractionText.gameObject.SetActive(false);
    }

}
