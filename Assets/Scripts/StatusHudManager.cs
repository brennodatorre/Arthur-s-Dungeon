using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

public class StatusHudManager : MonoBehaviour
{

    public RoundManager roundManager;

    public Image hpBar;
    public Image mpBar;
    public GameObject mainActionDisplay;
    public GameObject supActionDisplay;
    [HideInInspector] public Image mainActionDisplayImage;
    [HideInInspector] public Image supActionDisplayImage;
    [HideInInspector] public TooltipManager mainActionDisplayToolM;
    [HideInInspector] public TooltipManager supActionDisplayToolM;

    private int mainActionCount;
    private int supActionCount;

    private Color mainActionColor;
    private Color supActionColor;

    private void Awake()
    {
        mainActionDisplayImage = mainActionDisplay.GetComponent<Image>() ;
        supActionDisplayImage = supActionDisplay.GetComponent<Image>();
        mainActionDisplayToolM = mainActionDisplay.GetComponent<TooltipManager>();
        supActionDisplayToolM = supActionDisplay.GetComponent<TooltipManager>();



        //gets original colors of the action displays
        mainActionColor = mainActionDisplayImage.color;
        supActionColor = supActionDisplayImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the health and mana bars based on the player's current stats
        Entity player = roundManager.player;
        hpBar.fillAmount = player.getHP() / player.getMaxHP();
        mpBar.fillAmount = player.getMP() / player.getMaxMP();

        // Update the main action displays based on the player's current number of actions
        if (player.currentMainActions != mainActionCount)
        {
            mainActionDisplayToolM.description = "Main Actions: " + player.currentMainActions;
            if (player.currentMainActions < 1) { mainActionDisplayImage.color = Color.gray; }
            else { mainActionDisplayImage.color = mainActionColor; }
        }
        
        // Update the support action display
        if (player.currentSupActions != supActionCount)
        {
            supActionDisplayToolM.description = "Support Actions: " + player.currentSupActions;
            if (player.currentSupActions < 1) { supActionDisplayImage.color = Color.gray; }
            else { supActionDisplayImage.color = supActionColor; }
        }
        
        // updates memomry of main and sup actions
        mainActionCount = player.currentMainActions;
        supActionCount = player.currentSupActions;
        
    }
}
