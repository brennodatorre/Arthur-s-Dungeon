
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class ButtonManager : MonoBehaviour
{

    public static ButtonManager Instance;

    private RoundManager roundManager;
    private AudioManager audioManager;
    private SkillManager skillManager;
    private CursorManager cursorManager;
    private MySceneManager sceneManager;


    [SerializeField]
    public enum OnMenu
    {
        Action,
        Skill,
        Item, 
        None
    }

    [Space]
    [Header("GUI Elements")]
    public GameObject buttonPrefab;
    public GameObject itemButtonPrefab;
    public GameObject actMenu;
    public GameObject skillMenu;
    public GameObject skillMenuGrid;
    public GameObject itemMenu;
    public GameObject itemMenuGrid;

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
    public List<GameObject> skillButtons = new List<GameObject>(); 
    public List<GameObject> itemButtons = new List<GameObject>(); 
    public List<GameObject> actionButtons = new List<GameObject>();
    public GameObject lastButtonPressed;

    [Space]
    [Header("States")]
    public bool inAtkOverlay = false;
    public bool inSkillOverlay = false;
    public bool inItemOverlay = false;
    public OnMenu currentMenu; //the current menu of the turn


        void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }
    }


    void Start()
    {
        audioManager = AudioManager.Instance;
        
        cursorManager = CursorManager.Instance;

        sceneManager = MySceneManager.Instance;
        roundManager = RoundManager.Instance;
        skillManager = SkillManager.Instance;
    

    }


    private void Update()
    {
         

        if (MySceneManager.Instance.currentSceneType == MySceneManager.SceneType.TUTORIAL) { return; }
        if (sceneManager.currentSceneType == MySceneManager.SceneType.MAINMENU) { return; }
        
        if (roundManager == null) { roundManager = RoundManager.Instance; }
        if (roundManager == null ) { return; }
        
        
        bool canTriggerAtk = (currentMenu == OnMenu.Action || inAtkOverlay) && !inSkillOverlay && !inItemOverlay && roundManager.currentPhase != RoundManager.TurnPhase.Clash;
        bool canTriggerSkill = (currentMenu == OnMenu.Action || currentMenu == OnMenu.Skill || inSkillOverlay) && !inAtkOverlay  && !inItemOverlay && roundManager.currentPhase != RoundManager.TurnPhase.Clash;
        bool canTriggerItem = (currentMenu == OnMenu.Action || currentMenu == OnMenu.Item || inItemOverlay ) && !inAtkOverlay && !inSkillOverlay && roundManager.currentPhase != RoundManager.TurnPhase.Clash;
        bool canTriggerRun = currentMenu == OnMenu.Action && !roundManager.playerIsTargeting && roundManager.currentPhase != RoundManager.TurnPhase.Clash;

        


        //Dels with shortcut inputting
        if (roundManager != null && roundManager.playerCanAct)
        {
            //right mouse to cancel selection
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                //skill selection
                if (inSkillOverlay)
                {

                    inSkillOverlay = false;
                    unblockSkillButtons(skillButtons, lastButtonPressed.gameObject);
                    toggleBtns(true, skillButtons); //unlock the skill buttons
                    roundManager.toggleEntityTargetingUI(false); //disable the skill targetting UI
                    roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
                }
                //item selection
                else if (inItemOverlay)
                {
                    inItemOverlay = false;
                    unblockItemButtons(itemButtons, lastButtonPressed.gameObject);
                    toggleBtns(true, itemButtons); //unlock the skill buttons
                    roundManager.toggleEntityTargetingUI(false); //disable the skill targetting UI
                    roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
                }
            }
            if (Input.GetKeyDown(KeyCode.Q) && canTriggerAtk)
            {
                atkMenuButton(atk_button);
            }
            else if (Input.GetKeyDown(KeyCode.W) && canTriggerSkill)
            {
                //if not on skill menu, open it, else close it
                if (currentMenu != OnMenu.Skill) { skillMenuButton(); }
                else { closeSkillMenu(true); }
            }
            else if (Input.GetKeyDown(KeyCode.E) && canTriggerItem)
            {
                //if not on item menu, open it, else close it
                if (currentMenu != OnMenu.Item) { itemMenuButton(); }
                else { closeItemMenu(true); }
            }
            else if (Input.GetKeyDown(KeyCode.R) && canTriggerRun)
            {
                runMenuButton();
            }
        }



    }


    public void atkMenuButton(GameObject btn)
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
    public void skillMenuButton()
    {
        currentMenu = OnMenu.Skill;
        lastButtonPressed = skill_button;
        audioManager.PlaySkillButtonSound();
        openSkillMenu();
    }
    public void itemMenuButton()
    {
        currentMenu = OnMenu.Item;
        lastButtonPressed = item_button;
        audioManager.PlayItemButtonSound();
        openItemMenu();
    }
    public void runMenuButton()
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

        int childrenOfGrid = skillMenuGrid.transform.childCount;


        foreach (Skill skill in roundManager.currentTurn.skills)
        {
            //creates a button for each skill in the players skill list
            GameObject buttonObj = Instantiate(buttonPrefab, skillMenuGrid.transform);
            buttonObj.transform.SetSiblingIndex(childrenOfGrid); //puts the new button at the end of the liss (leaves background stuff on the back of the grid)
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName  + " (" + skill.mpCost + ") ";

            buttonObj.name = skill.skillName + "( SkillButton)";
            buttonObj.GetComponent<TooltipManager>().description = skill.description;
            buttonObj.GetComponent<TooltipManager>().tooltipPanel = tooltipPanel;
            buttonObj.GetComponent<TooltipManager>().detectChildren = true;
            buttonObj.GetComponent<TooltipManager>().tooltipText = tooltipText;
            buttonObj.GetComponent<TooltipManager>().cursorManager = cursorManager;
            buttonObj.GetComponent<TooltipManager>().btn = buttonObj;
            buttonObj.GetComponent<TooltipManager>().tooltipType = TooltipManager.TooltipType.Skill;
            buttonObj.AddComponent<DragAndDropItem>();
   


            //Sets the ball displayers based on the skill action type 
            // // as long as hierarchy does not change: Main (3), Sup (2), Bonus (1)
            if (skill.actionType == Skill.SkillActionType.Sup)
            {
                buttonObj.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                buttonObj.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            }
            else if (skill.actionType == Skill.SkillActionType.Main)
            {
                buttonObj.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                buttonObj.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            }

            
            

 
            Button button = buttonObj.GetComponent<Button>();

            button.onClick.AddListener(() => 
            {
                
                lastButtonPressed = buttonObj;

                skillButtons.Clear(); //clears the skill buttons list
                //gets all the buttons in the skill menu grid and adds them to the skillButtons list
                foreach (Transform child in skillMenuGrid.transform)
                {
                    if (child.GetComponent<Button>() != null) skillButtons.Add(child.gameObject);
                }

                if (!inSkillOverlay){
                    inSkillOverlay = true;
                    roundManager.skillSelected = skill;//tells skillManager which skill was selected

                    
                    blockSkillButtons(skillButtons, button.gameObject);
                    toggleBtns(false, skillButtons); //lock the skill buttons
                    lastButtonPressed.GetComponent<Button>().interactable = true; //unlock the last button pressed(the skill button that was pressed)

                    
                    roundManager.currentPhase = RoundManager.TurnPhase.targetingSKILL;//toggles skill targetting
                    
                    
                    roundManager.toggleEntityTargetingUI(true);


                }
                else {

                    inSkillOverlay = false;
                    unblockSkillButtons(skillButtons, button.gameObject);
                    toggleBtns(true, skillButtons); //unlock the skill buttons
                    roundManager.toggleEntityTargetingUI(false); //disable the skill targetting UI
                    roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
                }
            });
        }
        
    }

    public void openItemMenu()
    { 
        actMenu.SetActive(false);
        itemMenu.SetActive(true);

        //activateb the go back to action menu button
        backButton.SetActive(true);

        int childrenOfGrid = itemMenuGrid.transform.childCount;


        foreach (Item item in roundManager.currentTurn.items)
        {
            //creates a button for each skill in the players skill list
            GameObject buttonObj = Instantiate(itemButtonPrefab, itemMenuGrid.transform);
            buttonObj.transform.SetSiblingIndex(childrenOfGrid); //puts the new button at the end of the liss (leaves background stuff on the back of the grid)
            

            buttonObj.name = item.itemName + " (ItemButton)";
            buttonObj.GetComponent<TooltipManager>().description = item.description;
            buttonObj.GetComponent<TooltipManager>().tooltipPanel = tooltipPanel;
            buttonObj.GetComponent<TooltipManager>().detectChildren = true;
            buttonObj.GetComponent<TooltipManager>().tooltipText = tooltipText;
            buttonObj.GetComponent<TooltipManager>().cursorManager = cursorManager;
            buttonObj.GetComponent<TooltipManager>().btn = buttonObj;
            buttonObj.GetComponent<TooltipManager>().tooltipType = TooltipManager.TooltipType.Item;
            buttonObj.AddComponent<DragAndDropItem>();
            buttonObj.GetComponentInChildren<Image>().sprite = item.sprite;
            
        
 
            Button button = buttonObj.GetComponent<Button>();

            button.onClick.AddListener(() => 
            {
                if (item.isUsableInBattle)
                {
                    lastButtonPressed = buttonObj;

                    itemButtons.Clear(); //clears the skill buttons list
                    //gets all the buttons in the item menu grid and adds them to the skillButtons list
                    foreach (Transform child in itemMenuGrid.transform)
                    {
                        if (child.GetComponent<Button>() != null) itemButtons.Add(child.gameObject);
                    }

                    if (!inItemOverlay){
                        inItemOverlay = true;
                        roundManager.itemSelected = item;//tells roundManager which item was selected

                        
                        blockItemButtons(itemButtons, button.gameObject);
                        toggleBtns(false, itemButtons); //lock the item buttons
                        lastButtonPressed.GetComponent<Button>().interactable = true; //unlock the last button pressed(the item button that was pressed)

                        
                        roundManager.currentPhase = RoundManager.TurnPhase.targetingITEM;//toggles item targetting
                        
                        
                        roundManager.toggleEntityTargetingUI(true);


                    }
                    else {
                        inItemOverlay = false;
                        
                        unblockItemButtons(itemButtons, button.gameObject);
                        toggleBtns(true, itemButtons); //unlock the skill buttons
                        roundManager.toggleEntityTargetingUI(false); //disable the skill targetting UI
                        roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
                    }

                } else{}
                                

            });
        }
        
    }


    public void closeSkillMenu(bool withSound = false)
    {
        if (inSkillOverlay)
        {
            inSkillOverlay = false;
            roundManager.toggleEntityTargetingUI(false); //disable the skill targetting UI
            roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
        }

        if (withSound == true) { audioManager.PlaySkillButtonSound(); }

        // Clears the skill menu grid
        for (int i = skillMenuGrid.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = skillMenuGrid.transform.GetChild(i);
            if (child.gameObject.GetComponentInChildren<Button>() != null) { Destroy(child.gameObject); }
        }

        backButton.SetActive(false);
        skillMenu.SetActive(false);
        actMenu.SetActive(true);
        inSkillOverlay = false;
        currentMenu = OnMenu.Action;
    }

    public void closeItemMenu(bool withSound = false)
    {
        if (inItemOverlay)
        {
            inItemOverlay = false;
            roundManager.toggleEntityTargetingUI(false); //disable the item targetting UI
            roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
        }

        if (withSound == true) { audioManager.PlaySkillButtonSound(); }

        // Clears the item menu grid
        for (int i = itemMenuGrid.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = itemMenuGrid.transform.GetChild(i);
            if (child.gameObject.GetComponentInChildren<Button>() != null) { Destroy(child.gameObject); }
        }

        backButton.SetActive(false);
        itemMenu.SetActive(false);
        actMenu.SetActive(true);
        inItemOverlay = false;
        currentMenu = OnMenu.Action;
    }
    

    //togle the list of buttons passed 
    public void toggleBtns(bool switcher, List<GameObject> actionButtons)
    {

        foreach (GameObject actionBtn in actionButtons)
        {
            actionBtn.GetComponent<Button>().interactable = switcher;

        }

    }


    public void blockSkillButtons(List<GameObject> skillButtons, GameObject pressedBtn)
    {
        foreach (GameObject btn in skillButtons)
        {
            if (btn == pressedBtn) continue;

            CanvasGroup cg;

            if (btn.GetComponentInChildren<TextMeshProUGUI>() != null) btn.GetComponentInChildren<TextMeshProUGUI>().alpha = 0f;

            if (btn.GetComponent<CanvasGroup>() == null) btn.AddComponent<CanvasGroup>();
            cg = btn.GetComponent<CanvasGroup>();

            cg.alpha = .1f;


        }

        // moves the selected button up
        pressedBtn.transform.position += new Vector3(0, 5, 0);
        //sets glow outline to item selected
        //pressedBtn.GetComponentInChildren<Image>().material = roundManager.matPallet.getColoredMaterial(roundManager.matPallet.white, roundManager.matPallet.outlineSpriteMaterial);
    
    }
    public void unblockSkillButtons(List<GameObject> skillButtons, GameObject pressedBtn)
    {
        foreach (GameObject btn in skillButtons)
        {
            if (btn == pressedBtn) continue;

            CanvasGroup cg;

            if (btn.GetComponentInChildren<TextMeshProUGUI>() != null) btn.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;

            if (btn.GetComponent<CanvasGroup>() == null) btn.AddComponent<CanvasGroup>();
            cg = btn.GetComponent<CanvasGroup>();

            cg.alpha = 1f;
        }

        // moves the selected button back to original position
        pressedBtn.transform.position -= new Vector3(0, 5, 0);
        // removes glow outline from item selected
        //pressedBtn.GetComponentInChildren<Image>().material = roundManager.matPallet.getColoredMaterial(roundManager.matPallet.getItemOriginColor(roundManager.itemSelected), roundManager.matPallet.dissolveMaterial);
    }

    public void blockItemButtons(List<GameObject> itemButtons, GameObject pressedBtn)
    {
        foreach (GameObject btn in itemButtons)
        {
            if (btn == pressedBtn) continue;

            CanvasGroup cg;

    
            if (btn.GetComponent<CanvasGroup>() == null) btn.AddComponent<CanvasGroup>();
            cg = btn.GetComponent<CanvasGroup>();

            cg.alpha = .1f;


        }

        // moves the selected button up
        pressedBtn.transform.position += new Vector3(0, 5, 0);
        //sets glow outline to item selected
        pressedBtn.GetComponentInChildren<Image>().material = roundManager.matPallet.getColoredMaterial(roundManager.matPallet.white, roundManager.matPallet.outlineSpriteMaterial);

    }
    
        public void unblockItemButtons(List<GameObject> itemButtons, GameObject pressedBtn)
    {
        foreach (GameObject btn in itemButtons)
        {
            if (btn == pressedBtn) continue;

            CanvasGroup cg;

            if (btn.GetComponentInChildren<TextMeshProUGUI>() != null) btn.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;

            if (btn.GetComponent<CanvasGroup>() == null) btn.AddComponent<CanvasGroup>();
            cg = btn.GetComponent<CanvasGroup>();

            cg.alpha = 1f;
        }

        // moves the selected button back to original position
        pressedBtn.transform.position -= new Vector3(0, 5, 0);
        // removes glow outline from item selected
        pressedBtn.GetComponentInChildren<Image>().material = roundManager.matPallet.getColoredMaterial(roundManager.matPallet.getItemOriginColor(roundManager.itemSelected), roundManager.matPallet.dissolveMaterial);
    }

    public void closeAllMenus()
    {
        actMenu.SetActive(false);
        skillMenu.SetActive(false);
        itemMenu.SetActive(false);
    }  

   

}
