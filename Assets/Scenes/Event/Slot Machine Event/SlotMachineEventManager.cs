using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class SlotMachineEventManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;




    public string selectedSkill = "none";



    


    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
        

        updateSkillSlots();
    }

    void OnDropdownChanged(int index)
    {
        selectedSkill = dropdown.options[index].text;
        Debug.Log("Selected skill: " + selectedSkill);
        
    }


    public void updateSkillSlots()
    {
        dropdown.ClearOptions();

        dropdown.options.Add(new TMP_Dropdown.OptionData("CHOOSE SKILL"));

        foreach (Skill skl in PlayerData.Instance.getSkills())
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(skl.skillName));
        }

    }


    public void playSlotMachine()
    {
        if (selectedSkill == "none" || selectedSkill == "CHOOSE SKILL")
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.skill_unable_sound);

        }
        else
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.skill_button_sound);
            PlayerData.Instance.RemoveSkill(DatabaseManager.Instance.skillDatabase.GetSkill(selectedSkill));
            updateSkillSlots();
            selectedSkill = "none";

            int luck = PlayerData.Instance.GetTrait(PlayerData.Trait.LUCK);

            //                            N     i      s     T       I    h    P 
            if      (luck >= 6) {playOdds(0,    20,    25,   15,     10,  25,  5);}
            else if (luck >= 4) {playOdds(10,   15,    20,   10,     20,  20,  5);}
            else if (luck >= 2) {playOdds(20,   15,    15,   5,      20,  15,  10);}
            else if (luck >= 0) {playOdds(30,   10,    10,   5,      25,  10,  10);}
            else                {playOdds(40,   10,    5,    0,      30,  5,   10);}

        }


    }


    /// </summary>
    /// 
    /// decides the odds of the slot machine, based on the player's luck trait, sums up to 100
    /// 
    /// </summary>

    private void playOdds(int nothing, int item, int skill, int Status, int ilhas, int heal, int mpHeal)
    {
        int roll = Random.Range(0, 100);

        if (roll < nothing)
        {
            Debug.Log("You got nothing!");
            StartCoroutine(MySceneManager.Instance.doPopUp("You got nothing!", this.transform.position, Color.gray));
        }
        else if (roll < nothing + item) // item
        {
            Item newItem = DatabaseManager.Instance.itemDatabase.getRandom();
            PlayerData.Instance.addItemToInventory(newItem);

           
            StartCoroutine(MySceneManager.Instance.doPopUp("You got: " + newItem.itemName, this.transform.position, Color.yellow));
        }
        else if (roll < nothing + item + skill) // skill
        {
            Skill newSkill =DatabaseManager.Instance.skillDatabase.getRandom(PlayerData.Instance.getSkills());
            PlayerData.Instance.addSkill(newSkill);

            StartCoroutine(MySceneManager.Instance.doPopUp("You got: " + newSkill.skillName, this.transform.position, Color.cyan));
        }
        else if (roll < nothing + item + skill + Status) // status points
        {
            Debug.Log("You got 3 status points");
            StartCoroutine(MySceneManager.Instance.doPopUp("You got 3 status points!", this.transform.position, Color.magenta));
            PlayerData.Instance.changeStatusPoints(3);
        }
        else if (roll < nothing + item + skill + Status + ilhas) // ilhas
        {
            int ilhasToGet = (int)System.Math.Ceiling((double)PlayerData.Instance.getIlhas() / 5); // get 20% of the player's current ilhas, rounded up

            if (ilhasToGet == 0)
            {
                ilhasToGet = 1; // if the player has 0 ilhas, give them 1 instead
            }

            PlayerData.Instance.changeIlhas(ilhasToGet);

            
            StartCoroutine(MySceneManager.Instance.doPopUp("You got " + ilhasToGet + " ilhas!", this.transform.position, Color.yellow));
        }
        else if (roll < nothing + item + skill + Status + ilhas + heal) // full heal
        {
            PlayerData.Instance.healPlayer(999);

            
            StartCoroutine(MySceneManager.Instance.doPopUp("You got fullyhealed!", this.transform.position, Color.green));
        }
        else // mp heal
        {
            PlayerData.Instance.changeMP(999);

            Debug.Log("You got MP healed!");
            StartCoroutine(MySceneManager.Instance.doPopUp("You got fully MP healed!", this.transform.position, Color.blue));
        }

    }


}
