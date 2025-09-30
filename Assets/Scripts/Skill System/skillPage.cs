using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class skillPage : MonoBehaviour
{
    public Skill skill;
    public bool selected;
    
    [Space(2)]
    [Header("Page Symbols:")]
    public GameObject rose_symbol;
    public GameObject hex_symbol;
    public GameObject arthur_symbol;
    public GameObject unknown_symbol;
    public GameObject landreas_symbol;
    public GameObject survivor_symbol;
    public GameObject system_symbol;
    public GameObject flame_symbol;

    public void clickSP()
    {
        selected = !selected;
        FindObjectOfType<AudioManager>().PlaySkillPageSelectdSound();
        if (selected) { GetComponentInParent<Image>().color = Color.magenta; }
        else { GetComponentInParent<Image>().color = Color.black; }
    }

}
