using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class skillPage : MonoBehaviour
{
    public Skill skill;
    public bool selected;

    public void clickSP(){
        selected = !selected;
        FindObjectOfType<AudioManager>().PlaySkillPageSelectdSound();
        if (selected){ GetComponentInParent<Image>().color = Color.magenta;}
        else { GetComponentInParent<Image>().color = Color.white;}
    }

}
