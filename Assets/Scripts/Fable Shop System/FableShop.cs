
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;


public class FableShop : MonoBehaviour
{



    private SkillManager skillManager;
    private UpgradeManager upgradeManager;
    private CursorManager cursorManager;





    public int numberOfPagesOnShop = 1;

    [Tooltip("Chance to get a skill on the shop, 0-100")]
    [Range(0f, 1f)] public float chanceToGetUpgradePage = .25f; // chance to get a skill on the shop, [0-1]


    [Space(10)]
    [Header("Prefabs && Objects:")]
    public GameObject skillPagePrefab;
    public GameObject upgradePagePrefab;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public GameObject wallet;

    [Space(10)]
    public List<GameObject> PagesOnShop = new List<GameObject>();



    [Space(10)]
    [Header("Skills List:")]
    public List<Skill> unlockableSkills = new List<Skill>();
    public List<Skill> skillsOnShop = new List<Skill>();



    [Space(10)]
    [Header("Upgrades List:")]
    public List<Upgrade> possibleUpgrades = new List<Upgrade>();
    public List<Upgrade> upgradesOnShop = new List<Upgrade>();


    [Space(3)]
    public List<Skill> selectedSkills = new List<Skill>(); //player selected skills to be bought


    void Awake()
    {
        skillManager = SkillManager.Instance;
        cursorManager = CursorManager.Instance;
        upgradeManager = UpgradeManager.Instance;

    }

    void OnEnable()
    {


        // create copies of all skills the playes does not have and save them into unlockableSkills
        foreach (Skill skill in skillManager.skills)
        {
            if (!PlayerData.Instance.getSkills().Any(s => s.skillName == skill.skillName))
            {
                unlockableSkills.Add(Instantiate(skill));
            }
        }
        
        foreach (Upgrade upgrade in upgradeManager.upgrades)
        {

            possibleUpgrades.Add(Instantiate(upgrade));

        }




        // bound number of skill on avaible on shop by total the number of unlockable skills 
        //if (numberOfPagesOnShop > unlockableSkills.Count) { numberOfPagesOnShop = unlockableSkills.Count; }


        //gets n number of skills to be sold at the shop randomly
        for (int i = 0; i < numberOfPagesOnShop; i++)
        {
            float prob = Random.value; // [0,1]
            if (prob < chanceToGetUpgradePage)
            {
                // if the probability is met, add an upgrade page instead of a skill page
                int x = Random.Range(0, possibleUpgrades.Count);
                upgradesOnShop.Add(Instantiate(possibleUpgrades[x]));


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

        wallet.GetComponentInChildren<TextMeshProUGUI>().text = "Fable Points: " + PlayerData.Instance.getCurrentFablePoints().ToString();

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

            setSkillPageSymbol(skillPageObj.GetComponent<skillPage>()); // sets the symbol of the page based on the skill origin

            PagesOnShop.Add(skillPageObj);

        }
        foreach (Upgrade upg in upgradesOnShop)
        {
            //creates a button for each upgrade page in the shop
            GameObject upgradePageObj = Instantiate(upgradePagePrefab, this.transform);


            UpgradePage upPage = upgradePageObj.GetComponent<UpgradePage>();
            upPage.upgrade = upg; // sets the page on the object 


            //gets the description of the upgrade based on the type
            upgradePageObj.GetComponent<TooltipManager>().description = upg.upgradeDescription;
            upgradePageObj.GetComponent<TooltipManager>().tooltipPanel = tooltipPanel;
            upgradePageObj.GetComponent<TooltipManager>().tooltipText = tooltipText;
            upgradePageObj.GetComponent<TooltipManager>().cursorManager = cursorManager;
            upgradePageObj.GetComponent<TooltipManager>().btn = upgradePageObj;
            upgradePageObj.GetComponent<TooltipManager>().tooltipType = TooltipManager.TooltipType.Skill; //can be used for upgrade pages too

            GameObject upImgObj = upgradePageObj.transform.GetChild(0).gameObject;
            UnityEngine.UI.Image upgImage = upImgObj.GetComponent<UnityEngine.UI.Image>();
            upgImage.sprite = upPage.upgrade.image;
            upgImage.SetNativeSize();

            ///Set the offset of the image here///////
            //
            //////////

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
                amountDue += pageOb.GetComponent<UpgradePage>().upgrade.upgradeFableCost; // adds the cost of the upgrade to the total amount due
            }

        }


        if (PlayerData.Instance.getCurrentFablePoints() < amountDue) { Debug.Log("Not enough Fables"); }
        else
        { // adds the skills to player and leaves shop
            foreach (GameObject pageOb in PagesOnShop)
            {
                if (pageOb.GetComponent<skillPage>() != null && pageOb.GetComponent<skillPage>().selected)
                {
                    PlayerData.Instance.addSkill(pageOb.GetComponent<skillPage>().skill);
                    //PlayerData.Instance.loseFablePoints(amountDue);
                }
                else if (pageOb.GetComponent<UpgradePage>() != null && pageOb.GetComponent<UpgradePage>().selected)
                {
                    // if the page is an upgrade page, apply the upgrades
                    UpgradePage upgPage = pageOb.GetComponent<UpgradePage>();
                    upgradeManager.applyUpgrade(upgPage.upgrade.upgradeName); // applies the upgrade based on the type
                    //PlayerData.Instance.loseFablePoints( upgPage.upgrade.upgradeFableCost); // deducts the cost of the upgrade from the player's fable points
                }


            }
            PlayerData.Instance.loseFablePoints(amountDue);

            PlayerData.Instance.revitalizePlayer(); // resets the player data to their actual values
            FindObjectOfType<Or_Manager>().leaveOutsideReaderDomain();

        }
    }


    private void setSkillPageSymbol(skillPage page)
    {

        foreach (Transform child in page.transform)
        {
            child.gameObject.SetActive(false);
        }

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
