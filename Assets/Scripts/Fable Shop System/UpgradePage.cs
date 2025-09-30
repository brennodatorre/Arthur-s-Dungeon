

using UnityEngine;
using UnityEngine.UI;

public class UpgradePage : MonoBehaviour
{


    public Upgrade upgrade;
    public bool selected;
    private Color pageColor; // Default color for unselected pages\

    private void Awake()
    {
        pageColor = GetComponentInParent<Image>().color; // Store the default color of the page
    }

    public void clickPage(){
        selected = !selected;
        FindObjectOfType<AudioManager>().PlaySkillPageSelectdSound();
        if (selected) { GetComponentInParent<Image>().color = Color.magenta; }
        else { GetComponentInParent<Image>().color = pageColor; }
    }


    
}
