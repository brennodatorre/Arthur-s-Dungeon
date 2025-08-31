using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

public class StatusHudManager : MonoBehaviour
{
    public static StatusHudManager Instance;

    public RoundManager roundManager;


    public Image hpBar;
    public Image mpBar;
    public GameObject mainActionDisplay;
    public GameObject supActionDisplay;
    public GameObject livesDisplay;
    public GameObject LevelBeatenDisplay;
    [HideInInspector] public Image mainActionDisplayImage;
    [HideInInspector] public Image supActionDisplayImage;
    [HideInInspector] public TooltipManager mainActionDisplayToolM;
    [HideInInspector] public TooltipManager supActionDisplayToolM;


    // Variables to keep track of the number of actions and health/mana
    // These are used to update the display only when the values change
    private int mainActionCount;
    private int supActionCount;
    private int hpCount;
    private int mpCount;

    private Color mainActionColor;
    private Color supActionColor;


    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }

        mainActionDisplayImage = mainActionDisplay.GetComponent<Image>();
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
        hpBar.fillAmount = (float)player.getHP() / (float)player.getMaxHP();
        mpBar.fillAmount = (float)player.getMP() / (float)player.getMaxMP();

        // Update the tooltip descriptions for health and mana bars
        if (player.getHP() != hpCount)
        {
            hpCount = player.getHP();
            hpBar.GetComponentInParent<TooltipManager>().description = "HP: " + player.getHP() + "/" + player.getMaxHP();

        }
        // Update the mana bar tooltip
        if (player.getMP() != mpCount)
        {
            mpCount = player.getMP();
            mpBar.GetComponentInParent<TooltipManager>().description = "MP: " + player.getMP() + "/" + player.getMaxMP();
        }

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

    public void updateLivesCounterUI()
    {
        TooltipManager liveTM = livesDisplay.GetComponent<TooltipManager>();
        liveTM.description = "Lives " + (PlayerData.Instance.lives - PlayerData.Instance.death_counter) + " / " + PlayerData.Instance.lives;
    }

    public void updateLevelCounterUI()
    {
        LevelBeatenDisplay.GetComponent<TextMeshProUGUI>().text = "LEVEL: " + PlayerData.Instance.levelsBeat;
    }
}
