
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class StatusHudManager : MonoBehaviour
{
    public static StatusHudManager Instance;

    public RoundManager roundManager;


    public Image hpBar;
    public Image whiteHP;
    public Image mpBar;
    public Image whiteMP;
    public GameObject mainActionDisplay;
    public GameObject supActionDisplay;
    public GameObject livesDisplay;
    public GameObject LevelBeatenDisplay;
    public GameObject statusEffectPrefab;


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

    
    private List<GameObject> statusEffectIconList = new List<GameObject>();

    private bool removingIcons;

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



        // Update the tooltip descriptions for health and mana bars
        if (player.getHP() != hpCount)
        {
            float hpRatio = (float)player.getHP() / (float)player.getMaxHP();
            hpBar.fillAmount = hpRatio;
            StartCoroutine(AnimationManager.Instance.doBarChangeAnimation(whiteHP, hpRatio));

            hpCount = player.getHP();
            hpBar.GetComponentInParent<TooltipManager>().description = "HP: " + player.getHP() + " / " + player.getMaxHP();

        }
        // Update the mana bar tooltip
        if (player.getMP() != mpCount)
        {
            float mpRatio = (float)player.getMP() / (float)player.getMaxMP();
            mpBar.fillAmount = mpRatio;
            StartCoroutine(AnimationManager.Instance.doBarChangeAnimation(whiteMP, mpRatio));

            mpCount = player.getMP();
            mpBar.GetComponentInParent<TooltipManager>().description = "MP: " + player.getMP() + " / " + player.getMaxMP();
        }

        // Update the main action displays based on the player's current number of actions
        if (player.currentMainActions != mainActionCount)
        {
            mainActionDisplayToolM.description = "Main Actions: " + player.currentMainActions;
            if (player.currentMainActions < 1) { StartCoroutine(AnimationManager.Instance.DissolveUponDeath(mainActionDisplayImage)); }
            else { StartCoroutine(AnimationManager.Instance.DissolveUponDeath(mainActionDisplayImage, true)); }
        }

        // Update the support action display
        if (player.currentSupActions != supActionCount)
        {
            supActionDisplayToolM.description = "Support Actions: " + player.currentSupActions;
            if (player.currentSupActions < 1) { StartCoroutine(AnimationManager.Instance.DissolveUponDeath(supActionDisplayImage)); }
            else { StartCoroutine(AnimationManager.Instance.DissolveUponDeath(supActionDisplayImage, true)); }
        }

        

        // updates memomry
        mainActionCount = player.currentMainActions;
        supActionCount = player.currentSupActions;
        

    }

    public void updateLivesCounterUI()
    {
        TooltipManager liveTM = livesDisplay.GetComponent<TooltipManager>();
        liveTM.description = "Lives " + (PlayerData.Instance.getLives() - PlayerData.Instance.getDeathCounter()) + " / " + PlayerData.Instance.getLives();
    }

    public void updateLevelCounterUI()
    {
        LevelBeatenDisplay.GetComponent<TextMeshProUGUI>().text = "LEVEL: " + PlayerData.Instance.getLevelsBeat() + " \n" + "High Score: "+ PlayerData.Instance.GetHighestScore();
    }

    public GameObject addStatusEffectToDisplay(StatusEffect stat)
    {
        GameObject prefab = Instantiate(statusEffectPrefab, stat.target.statEffectDisplay.transform);
        statusEffectIconList.Add(prefab);

        if (stat.target == roundManager.player) prefab.transform.localScale = Vector3.one; // set the scale of the prefab to match the scale of the status effect display 

        // prefab.GetComponent<TooltipManager>().tooltipPanel = MySceneManager.Instance.tooltipPanel;
        prefab.GetComponent<TooltipManager>().canvas = MySceneManager.Instance.canvas;

        prefab.GetComponent<StatusEffectIcon>()._statusEffect = stat;

        prefab.GetComponent<Image>().sprite = stat.sprite;
        prefab.GetComponentInChildren<TooltipManager>().description = stat.effectName + " " + stat.currentDuration + " / " + stat.duration + "\n\n" + stat.description;

        return prefab;

    }

    public void UpdateStatusEffectDisplay()
    {
        List<GameObject> statusUpdate = new List<GameObject>(statusEffectIconList);

        foreach (GameObject st in statusUpdate)
        {
            if (st == null)
                continue;

            StatusEffectIcon icon = st.GetComponent<StatusEffectIcon>();
            TooltipManager tooltip = st.GetComponentInChildren<TooltipManager>();

            StatusEffect stat = icon._statusEffect;

            tooltip.description =
                stat.effectName + " " +
                stat.currentDuration + " / " +
                stat.duration + "\n\n" +
                stat.description;
        }


    }


   public void RemoveIcon(GameObject icon)
    {
        statusEffectIconList.Remove(icon);
    }


}
