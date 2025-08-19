
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;

public class FableShop : MonoBehaviour
{



    private SkillManager skillManager;
    private CursorManager cursorManager;


    public int numberOfPagesOnShop = 1;

    [Tooltip("Chance to get a skill on the shop, 0-100")]
    public float chanceToGetUpgradePage = 25; // chance to get a skill on the shop, 0-100
    

    [Space(1)]
    [Header("Prefabs && Objects:")]
    public GameObject skillPagePrefab;
    public GameObject upgradePagePrefab;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public GameObject wallet;



    [Space(3)]
    [Header("Skills List:")]
    public List<Skill> unlockableSkills = new List<Skill>();
    public List<Skill> skillsOnShop = new List<Skill>();
    public int upgradePagesOnShop ; // list of upgrade pages on the shop
    public List<GameObject> PagesOnShop = new List<GameObject>();

    [Space(3)]
    public List<Skill> selectedSkills = new List<Skill>(); //player selected skills to be bought


    void Awake()
    {
        skillManager = SkillManager.Instance;
        cursorManager = CursorManager.Instance;

    }

    void OnEnable()
    {


        // create copies of all skills the playes does not have and save them into unlockableSkills
        foreach (Skill skill in skillManager.skills)
        {
            if (!PlayerData.Instance.skills.Any(s => s.skillName == skill.skillName))
            {
                unlockableSkills.Add(Instantiate(skill));
            }
        }




        // bound number of skill on avaible on shop by total the number of unlockable skills 
        //if (numberOfPagesOnShop > unlockableSkills.Count) { numberOfPagesOnShop = unlockableSkills.Count; }
        

        //gets n number of skills to be sold at the shop randomly
        for (int i = 0; i < numberOfPagesOnShop; i++)
        {
            int prob = Random.Range(0, 100);
            if (prob > chanceToGetUpgradePage)
            {
                // if the probability is met, add an upgrade page instead of a skill page
                upgradePagesOnShop++;

            }
            else
            {
                // otherwise, add a random skill from the unlockable skills list
                int x = Random.Range(0, unlockableSkills.Count);
                skillsOnShop.Add(Instantiate(unlockableSkills[x]));
                unlockableSkills.RemoveAt(x);
            }



        }



    }


    void Start()
    {

        wallet.GetComponentInChildren<TextMeshProUGUI>().text = "Fable Points: " + PlayerData.Instance.fablePoints.ToString();

        foreach (Skill skl in skillsOnShop)
        {
            //creates a button for each skillpage in the shop
            GameObject skillPageObj = Instantiate(skillPagePrefab, this.transform);


            skillPageObj.GetComponent<TooltipManager>().description =
                                                        "Cost: " + skl.fableCost + "\n" +
                                                        skl.skillName + "\n" +
                                                        skl.description;
            skillPageObj.GetComponent<TooltipManager>().tooltipPanel = tooltipPanel;
            skillPageObj.GetComponent<TooltipManager>().tooltipText = tooltipText;
            skillPageObj.GetComponent<TooltipManager>().cursorManager = cursorManager;
            skillPageObj.GetComponent<TooltipManager>().btn = skillPageObj;
            skillPageObj.GetComponent<TooltipManager>().tooltipType = TooltipManager.TooltipType.Skill;
            skillPageObj.GetComponent<skillPage>().skill = skl;

            setPageSymbol(skillPageObj.GetComponent<skillPage>()); // sets the symbol of the page based on the skill origin

            PagesOnShop.Add(skillPageObj);

        }
        for (int i = 0; i < upgradePagesOnShop; i++)
        {
            //creates a button for each upgrade page in the shop
            GameObject upgradePageObj = Instantiate(upgradePagePrefab, this.transform);
            

            UpgradePage upPage= upgradePageObj.GetComponent<UpgradePage>();
            upPage.setRandomPage(); // sets the page on the object 


            //gets the description of the upgrade based on the type
            upgradePageObj.GetComponent<TooltipManager>().description = upPage.upgradeDescription;
            upgradePageObj.GetComponent<TooltipManager>().tooltipPanel = tooltipPanel;
            upgradePageObj.GetComponent<TooltipManager>().tooltipText = tooltipText;
            upgradePageObj.GetComponent<TooltipManager>().cursorManager = cursorManager;
            upgradePageObj.GetComponent<TooltipManager>().btn = upgradePageObj;
            upgradePageObj.GetComponent<TooltipManager>().tooltipType = TooltipManager.TooltipType.Skill; //can be used for upgrade pages too

            PagesOnShop.Add(upgradePageObj);
        }

    }


    public void closeShop()
    {

        int amountDue = 0;

        // calculates the total amount due
        foreach (GameObject pageOb in PagesOnShop)
        {
            if (pageOb.GetComponent<skillPage>() != null && pageOb.GetComponent<skillPage>().selected)
            {
                amountDue += pageOb.GetComponent<skillPage>().skill.fableCost;
            }
            else if (pageOb.GetComponent<UpgradePage>() != null && pageOb.GetComponent<UpgradePage>().selected)
            { 
                amountDue += pageOb.GetComponent<UpgradePage>().upgradeFableCost; // adds the cost of the upgrade to the total amount due
            }

        }


        if (PlayerData.Instance.fablePoints < amountDue) { Debug.Log("Not enough Fables"); }
        else
        { // adds the skills to player and leaves shop
            foreach (GameObject pageOb in PagesOnShop)
            {
                if (pageOb.GetComponent<skillPage>() != null && pageOb.GetComponent<skillPage>().selected)
                {
                    PlayerData.Instance.skills.Add(pageOb.GetComponent<skillPage>().skill);
                    PlayerData.Instance.fablePoints -= amountDue;
                }
                else if (pageOb.GetComponent<UpgradePage>() != null && pageOb.GetComponent<UpgradePage>().selected)
                {
                    // if the page is an upgrade page, apply the upgrades
                    UpgradePage upgPage = pageOb.GetComponent<UpgradePage>();
                    upgPage.applyUpgrade(upgPage.upgradeType); // applies the upgrade based on the type
                    PlayerData.Instance.fablePoints -= upgPage.upgradeFableCost; // deducts the cost of the upgrade from the player's fable points
                }
                

            }

            PlayerData.Instance.revitalizePlayer(); // resets the player data to their actual values
            FindObjectOfType<Or_Manager>().leaveOutsideReaderDomain();

        }
    }


    private void setPageSymbol(skillPage page)
    { 

        // Deactivate all symbols first
        page.rose_symbol.SetActive(false);
        page.hex_symbol.SetActive(false);
        page.arthur_symbol.SetActive(false);
        page.landreas_symbol.SetActive(false);
        page.survivor_symbol.SetActive(false);
        page.system_symbol.SetActive(false);
        page.flame_symbol.SetActive(false);
        page.unknown_symbol.SetActive(false);

        // Activate the symbol based on the skill's origin
        switch (page.skill.origin)
        {
            case Skill.SkillOrigin.HEX:
                page.hex_symbol.SetActive(true);
                break;
            case Skill.SkillOrigin.ARTHUR:
                page.arthur_symbol.SetActive(true);
                break;
            case Skill.SkillOrigin.LANDREAS:
                page.landreas_symbol.SetActive(true);
                break;
            case Skill.SkillOrigin.SURVIVOR:
                page.survivor_symbol.SetActive(true);
                break;
            case Skill.SkillOrigin.SYSTEM:
                page.system_symbol.SetActive(true);
                break;
            case Skill.SkillOrigin.FLAME:
                page.flame_symbol.SetActive(true);
                break;
            case Skill.SkillOrigin.UNKNOWN:
                page.unknown_symbol.SetActive(true);
                break;
            case Skill.SkillOrigin.ROSES:
                page.rose_symbol.SetActive(true);
                break;
            default:
                Debug.Log("Skill origin not recognized");
                break;
        }
    }

}
