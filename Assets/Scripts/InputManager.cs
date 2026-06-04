using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    RoundManager roundManager;

    ButtonManager btnManager;

    public bool inGodLikeMode = false;
    [SerializeField] Entity GodLikePlayerData ;
    [SerializeField] Entity originalPlayerData ;

    

    void Start()
    {
        if (RoundManager.Instance != null)
        {
            roundManager = RoundManager.Instance;
        }
        if (ButtonManager.Instance != null)
        {
            btnManager = ButtonManager.Instance;
        }
    }


    
    void Update() {

        // toggle god like player data for testing purposes
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.D) && roundManager != null)
        {
            if (inGodLikeMode)
            {
                Debug.Log("Reverting to original player data");
                roundManager.player.CopyFrom(originalPlayerData);

            }
            else
            {
                Debug.Log("Switching to god like player data");
                originalPlayerData.CopyFrom(roundManager.player);
                roundManager.player.CopyFrom(GodLikePlayerData);
                roundManager.player.skills = new List<Skill>(DatabaseManager.Instance.allSkillsDatabase.skills);


            }
            

            inGodLikeMode = !inGodLikeMode;
        }
        
        // Info Menu
        else if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            InfoDisplayManager.Instance.ToggleInfoMenu();
        }


        // Act Menu 
        else if (roundManager != null && roundManager.playerCanAct)
        {
            //right mouse to cancel selection
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                //skill selection
                if (btnManager.inSkillOverlay)
                {

                    btnManager.inSkillOverlay = false;
                    btnManager.unblockSkillButtons(btnManager.skillButtons, btnManager.lastButtonPressed.gameObject);
                    btnManager.toggleBtns(true, btnManager.skillButtons); //unlock the skill buttons
                    roundManager.toggleEntityTargetingUI(false); //disable the skill targetting UI
                    roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
                }
                //item selection
                else if (btnManager.inItemOverlay)
                {
                    btnManager.inItemOverlay = false;
                    btnManager.unblockItemButtons(btnManager.itemButtons, btnManager.lastButtonPressed.gameObject);
                    btnManager.toggleBtns(true, btnManager.itemButtons); //unlock the item buttons
                    roundManager.toggleEntityTargetingUI(false); //disable the item targetting UI
                    roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
                }
            }




            if (Input.GetKeyDown(KeyCode.Q) && btnManager.canTriggerAtk)
            {
                btnManager.atkMenuButton(btnManager.atk_button);
            }
            else if (Input.GetKeyDown(KeyCode.W) && btnManager.canTriggerSkill)
            {
                //if not on skill menu, open it, else close it
                if (btnManager.currentMenu != ButtonManager.OnMenu.Skill) { btnManager.skillMenuButton(); }
                else { btnManager.closeSkillMenu(true); }
            }
            else if (Input.GetKeyDown(KeyCode.E) && btnManager.canTriggerItem)
            {
                //if not on item menu, open it, else close it
                if (btnManager.currentMenu != ButtonManager.OnMenu.Item) { btnManager.itemMenuButton(); }
                else { btnManager.closeItemMenu(true); }
            }
            else if (Input.GetKeyDown(KeyCode.R) && btnManager.canTriggerRun)
            {
                btnManager.runMenuButton();
            }
        }






    }

    
}
