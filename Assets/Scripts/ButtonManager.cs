using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ButtonManager : MonoBehaviour
{
    private RoundManager roundManager;
    private AudioManager audioManager;
    private SkillManager skillManager;
    private CursorManager cursorManager;
    private MySceneManager sceneManager;


    [SerializeField] public enum OnMenu
    {
        Action,
        Skill,
        Item 
    }

    [Space]
    [Header("GUI Elements")]
    public GameObject buttonPrefab;
    public GameObject actMenu;
    public GameObject skillMenu;
    public GameObject skillMenuGrid;
    public GameObject itemMenu;
    //public GameObject itemMenuGrid;

    [Space]
    [Header("Buttons")]
    public GameObject atk_button;
    public GameObject skill_button;
    public GameObject item_button;
    public GameObject run_button;
    public GameObject backButton;

    [Space]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    
    [Space]
    [HideInInspector] public List<GameObject> skillButtons = new List<GameObject>(); 
    public List<GameObject> actionButtons = new List<GameObject>();
    public GameObject lastButtonPressed;

    [Space]
    [Header("States")]
    public bool inAtkOverlay = false;
    public bool inSkillOverlay = false;
    public bool inItemOverlay = false;
    public OnMenu currentMenu; //the current menu of the turn




    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        
        cursorManager = GameObject.Find("CursorManager").GetComponent<CursorManager>();

        sceneManager = GameObject.Find("MySceneManager").GetComponent<MySceneManager>();

        if (sceneManager.sceneType == MySceneManager.SceneType.COMBAT) 
        {
            roundManager = GameObject.Find("CombatManager").GetComponent<RoundManager>();
            skillManager = GameObject.Find("SkillManager").GetComponent<SkillManager>();
        }

    }


    private void Update()
    {
        //if its on the player's turn and they havent ended their action
        if (roundManager != null && roundManager.currentTurn == roundManager.player && roundManager.currentTurn.currentMainActions > 0 && (roundManager.currentPhase == RoundManager.TurnPhase.Action|| roundManager.currentPhase == RoundManager.TurnPhase.targetingATK))
        {
            if (Input.GetKeyDown(KeyCode.Q)  && !roundManager.playerIsAttacking && currentMenu != OnMenu.Skill && currentMenu != OnMenu.Item)
            {
                ATK_button(atk_button);
            }
            else if (Input.GetKeyDown(KeyCode.W) && !inAtkOverlay && currentMenu != OnMenu.Item)
            {
                //if not on skill already open skill menu, else close it
                if (currentMenu != OnMenu.Skill) { SKILL_button(); }
                else
                {
                    audioManager.PlaySkillButtonSound();
                    closeSkillMenu();
                }
            }
            else if (Input.GetKeyDown(KeyCode.E) && !inAtkOverlay && currentMenu != OnMenu.Skill)
            {
                ITEM_button();
            }
            else if (Input.GetKeyDown(KeyCode.R) && !inAtkOverlay && currentMenu != OnMenu.Skill && currentMenu != OnMenu.Item)
            {
                RUN_button();
            }
        }



    }


    public void ATK_button(GameObject btn)
    {
        lastButtonPressed = btn;

        audioManager.PlayAtkButtonSound();

        if (!inAtkOverlay)
        {

            inAtkOverlay = true;
            roundManager.currentPhase = RoundManager.TurnPhase.targetingATK;

            //lock the other action buttons
            toggleBtns(false, actionButtons);
            // but not the last button pressed(atk button in this case)
            lastButtonPressed.GetComponent<Button>().interactable = true;

            roundManager.EnableEnemyTargetingUI(true);
        }
        else
        {
            inAtkOverlay = false;
            //unlock the other action buttons
            toggleBtns(true, actionButtons);

            roundManager.EnableEnemyTargetingUI(false);
            roundManager.currentPhase = RoundManager.TurnPhase.Action;

        }

    }
    public void SKILL_button()
    {
        currentMenu = OnMenu.Skill;
        lastButtonPressed = skill_button;
        audioManager.PlaySkillButtonSound();
        openSkillMenu();
    }
    public void ITEM_button()
    {
        //currentMenu = OnMenu.Item;
        lastButtonPressed = item_button;
        audioManager.PlayItemButtonSound();
        //openItemMenu();
    }
    public void RUN_button()
    {
        lastButtonPressed = run_button;
        audioManager.PlayRunButtonSound();
        //run away from battle
    }


    private void openSkillMenu()
    {

        

        actMenu.SetActive(false);
        skillMenu.SetActive(true);

        //activateb the go back to action menu button
        backButton.SetActive(true);
        



        foreach (Skill skill in roundManager.currentTurn.skillsInstance)
        {
            //creates a button for each skill in the players skill list
            GameObject buttonObj = Instantiate(buttonPrefab, skillMenuGrid.transform);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName  + " (" + skill.mpCost + ") ";

            buttonObj.GetComponent<TooltipManager>().description = skill.description;
            buttonObj.GetComponent<TooltipManager>().tooltipPanel = tooltipPanel;
            buttonObj.GetComponent<TooltipManager>().tooltipText = tooltipText;
            buttonObj.GetComponent<TooltipManager>().cursorManager = cursorManager;
            buttonObj.GetComponent<TooltipManager>().btn = buttonObj;
            buttonObj.GetComponent<TooltipManager>().tooltipType = TooltipManager.TooltipType.Skill;

            //Sets the ball displayers based on the skill action type 
            // // as long as hierarchy does not change: Main (3), Sup (2), Bonus (1)
            if (skill.actionType == Skill.SkillActionType.Sup) {
                buttonObj.transform.GetChild(1).GetChild(1).gameObject.SetActive(false); 
                buttonObj.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);            }
            else if (skill.actionType == Skill.SkillActionType.Main) {
                buttonObj.transform.GetChild(1).GetChild(1).gameObject.SetActive(false); 
                buttonObj.transform.GetChild(1).GetChild(3).gameObject.SetActive(true);  }

            
            

 
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => 
            {
                
                lastButtonPressed = buttonObj;

                skillButtons.Clear(); //clears the skill buttons list
                //gets all the buttons in the skill menu grid and adds them to the skillButtons list
                foreach (Transform child in skillMenuGrid.transform)
                {
                    skillButtons.Add(child.gameObject);
                }

                if (!inSkillOverlay){
                    inSkillOverlay = true;
                    toggleBtns(false, skillButtons); //lock the skill buttons
                    lastButtonPressed.GetComponent<Button>().interactable = true; //unlock the last button pressed(the skill button that was pressed)

                    //toggles skill targetting
                    roundManager.currentPhase = RoundManager.TurnPhase.targetingSKILL;
                    roundManager.EnableSkillTargetingUI(true);

                    //tells skillManager which skill was selected
                    roundManager.skillSelected = skill;
                }
                else {
                    inSkillOverlay = false;
                    toggleBtns(true, skillButtons); //unlock the skill buttons
                    roundManager.EnableSkillTargetingUI(false); //disable the skill targetting UI
                    roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
                }
            });
        }
        
    } 


    public void closeSkillMenu()
    {
        // Clears the skill menu grid
        for (int i = skillMenuGrid.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = skillMenuGrid.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        backButton.SetActive(false);
        skillMenu.SetActive(false);
        actMenu.SetActive(true);
        inSkillOverlay = false;
        currentMenu = OnMenu.Action;
    }
    

    //togle the list of buttons passed 
    public void toggleBtns(bool switcher, List<GameObject> actionButtons )
    {
        

        foreach (GameObject actionBtn in actionButtons)
        {
            actionBtn.GetComponent<Button>().interactable = switcher;
        }

    }
    


}
