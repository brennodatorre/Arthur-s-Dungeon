
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;

public class FableShop : MonoBehaviour
{

    public SkillManager skillManager;
    public CursorManager cursorManager;
    public GameObject skillPagePrefab;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    public GameObject wallet;

    public int numberOfSkillsOnShop= 1;

    public List<Skill> unlockableSkills = new List<Skill>();
    public List<Skill> skillsOnShop = new List<Skill>();
    public List<GameObject> skillsPagesOnShop = new List<GameObject>();

    [Space]
    public List<Skill> selectedSkills = new List<Skill>(); //player selected skills to be bought
    

    void OnEnable()
    {
        
        
        // create copies of all skills the playes does not have and save them into unlockableSkills
        foreach (Skill skill in skillManager.skills){
            if (!PlayerData.Instance.skills.Any(s => s.skillName == skill.skillName))
            {
                unlockableSkills.Add(Instantiate(skill));
            }
        }

        // bound number of skill on avaible on shop by total the number of unlockable skills 
        if (numberOfSkillsOnShop > unlockableSkills.Count) {numberOfSkillsOnShop = unlockableSkills.Count;}
        //gets n number of skills to be sold at the shop randomly
        for (int i = 0; i < numberOfSkillsOnShop; i++){
            int x = Random.Range(0, unlockableSkills.Count);
            skillsOnShop.Add( Instantiate(unlockableSkills[x]));
            unlockableSkills.RemoveAt(x);
        }



    }

    // Start is called before the first frame update
    void Start()
    {

        wallet.GetComponentInChildren<TextMeshProUGUI>().text = "Fable Points: " + PlayerData.Instance.fablePoints.ToString();

        foreach (Skill skl in skillsOnShop){
            //creates a button for each skillpage in the shop
            GameObject skillPageObj = Instantiate(skillPagePrefab, this.transform);
            

            skillPageObj.GetComponent<TooltipManager>().description = 
                                                        "Cost: " +skl.fableCost+ "\n" + 
                                                        skl.skillName+ "\n" + 
                                                        skl.description;
            skillPageObj.GetComponent<TooltipManager>().tooltipPanel = tooltipPanel;
            skillPageObj.GetComponent<TooltipManager>().tooltipText = tooltipText;
            skillPageObj.GetComponent<TooltipManager>().cursorManager = cursorManager;
            skillPageObj.GetComponent<TooltipManager>().btn = skillPageObj;
            skillPageObj.GetComponent<TooltipManager>().hasDescription = true;
            skillPageObj.GetComponent<skillPage>().skill = skl;

            skillsPagesOnShop.Add(skillPageObj);

        }

    }


    public void closeShop(){

        int amountDue= 0;

        // calculates the total amount due
        foreach (GameObject pageOb in skillsPagesOnShop)
        {
            if (pageOb.GetComponent<skillPage>().selected)
            {
                amountDue += pageOb.GetComponent<skillPage>().skill.fableCost;
            }
        }


        if (PlayerData.Instance.fablePoints < amountDue) { Debug.Log("Not enough Fables"); }
        
        else
        { // adds the skills to player and leaves shop
            foreach (GameObject pageOb in skillsPagesOnShop)
            {
                if (pageOb.GetComponent<skillPage>().selected)
                {
                    PlayerData.Instance.skills.Add(pageOb.GetComponent<skillPage>().skill);
                    PlayerData.Instance.fablePoints -= amountDue;
                }

            }
            PlayerData.Instance.revivePlayer();
            FindObjectOfType<Or_Manager>().leaveOutsideReaderDomain();

        }
    }

   
}
