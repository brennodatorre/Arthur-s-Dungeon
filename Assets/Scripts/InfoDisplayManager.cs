using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class InfoDisplayManager : MonoBehaviour
{
    public static InfoDisplayManager Instance;

    public GameObject infoMenu;

    [Space(10)]
    public GameObject mainInfoDisplay;
    public GameObject SkillsDisplay;
    public GameObject ItemDisplay;

    [Space(10)]
    public GameObject mainDisplayButton;
    public GameObject skillsDisplayButton;
    public GameObject itemDisplayButton;

    #region MID Stats
    [Header("Main Info Display")]
    [Space(15)]
    public GameObject hpDisplay;
    public GameObject mpDisplay;
    public GameObject defDisplay;
    public GameObject atkDisplay;
    public GameObject ilhasDisplay;
    public GameObject fableDisplay;

    [Space(10)]
    public GameObject dexDisplay;
    public GameObject atlDisplay;
    public GameObject auraDisplay;
    public GameObject intuitionDisplay;
    public GameObject luckDisplay;
    public GameObject charDisplay;
    public GameObject hexDisplay;
    public GameObject intDisplay;
    public GameObject willDisplay;
    public GameObject furtDisplay;
    public GameObject persptDisplay;
    public GameObject reflxDisplay;
    public GameObject constDisplay;
    public GameObject domDiaplay;
    #endregion

    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        
    }


    void Start()
    {
        


        var skl  = SkillsDisplay.GetComponentInChildren<MenuContainerManager>();

        foreach (var skill in PlayerData.Instance.getSkills())
        {
            var temp = skl.AddItem(skill.skillName, () => Debug.Log("Skill Clicked!"));

            temp.GetComponent<TooltipManager>().description = skill.GetFullDescription();

            temp.GetComponent<Button>().onClick.AddListener(() => {
                EventManager.Instance?.InputSkill(skill);
            });
        }

        var itm = ItemDisplay.GetComponentInChildren<MenuContainerManager>();

        foreach (var item in PlayerData.Instance.getItems())
        {
            var temp = itm.AddItem(item.itemName, () => Debug.Log("Item Clicked!"));

            temp.GetComponent<TooltipManager>().description = item.GetFullDescription();

            temp.GetComponent<Button>().onClick.AddListener(() => {
                EventManager.Instance?.InputItem(item);
            });
        }


        setTabButtons();

    }




   

    public void ToggleInfoMenu()
    {
        

        infoMenu.SetActive(!infoMenu.activeSelf);

        if (infoMenu.activeSelf)
        {
            //open the main info tab by default
            mainInfoDisplay.SetActive(true);
            SkillsDisplay.SetActive(false);
            ItemDisplay.SetActive(false);
            // Update the info menu with the player's current stats
            setPlayerInfo();



            
        }
    }


    private void setTabButtons()
    {
        
            

            //set buttons for display switching
            mainDisplayButton.GetComponent<Image>().color = Color.white;
            skillsDisplayButton.GetComponent<Image>().color = Color.grey;
            itemDisplayButton.GetComponent<Image>().color = Color.grey;


            mainDisplayButton.GetComponent<Button>().onClick.AddListener(() => {
                mainInfoDisplay.SetActive(true);
                SkillsDisplay.SetActive(false);
                ItemDisplay.SetActive(false);


                mainDisplayButton.GetComponent<Image>().color = Color.white;
                skillsDisplayButton.GetComponent<Image>().color = Color.grey;
                itemDisplayButton.GetComponent<Image>().color = Color.grey;
            });

            skillsDisplayButton.GetComponent<Button>().onClick.AddListener(() => {
                mainInfoDisplay.SetActive(false);
                SkillsDisplay.SetActive(true);
                ItemDisplay.SetActive(false);

                mainDisplayButton.GetComponent<Image>().color = Color.grey;
                skillsDisplayButton.GetComponent<Image>().color = Color.white;
                itemDisplayButton.GetComponent<Image>().color = Color.grey;
            });

            itemDisplayButton.GetComponent<Button>().onClick.AddListener(() => {
                mainInfoDisplay.SetActive(false);
                SkillsDisplay.SetActive(false);
                ItemDisplay.SetActive(true);

                mainDisplayButton.GetComponent<Image>().color = Color.grey;
                skillsDisplayButton.GetComponent<Image>().color = Color.grey;
                itemDisplayButton.GetComponent<Image>().color = Color.white;
            });

            
    }


    private void setPlayerInfo(){
        PlayerData pdata = PlayerData.Instance;
            if (pdata != null)
            {
                if (MySceneManager.Instance.currentSceneType == MySceneManager.SceneType.COMBAT)
                {
                    Entity player = RoundManager.Instance.player;
                    hpDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "HP: " + player.getHP() + "/" + player.getMaxHP();
                    mpDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "MP: " + player.getMP() + "/" + player.getMaxMP();
                }
                else
                {
                    hpDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "HP: " + pdata.getCurrentHP()  + "/" + pdata.getMaxHP();;
                    mpDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "MP: " + pdata.getCurrentMP() + "/" + pdata.getMaxMP();
                }
                


                defDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "DEF: " + pdata.getDEF();
                atkDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "ATK: " + pdata.getBaseATK().ToString();
                ilhasDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "ILHAS: " + pdata.getIlhas();
                fableDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "FABLE: " + pdata.getCurrentFablePoints();

                dexDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "DEX: " + pdata.GetTrait(Trait.DEX);
                atlDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "ATL: " + pdata.GetTrait(Trait.ATLETISM);
                auraDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "AURA: " + pdata.GetTrait(Trait.AURA);
                intuitionDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "OMEN: " + pdata.GetTrait(Trait.INTUITION);
                luckDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "LUCK: " + pdata.GetTrait(Trait.LUCK);
                charDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "CHAR: " + pdata.GetTrait(Trait.CHARISM);
                hexDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "HEX: " + pdata.GetTrait(Trait.HEX);
                intDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "INT: " + pdata.GetTrait(Trait.INT);
                willDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "WILL: " + pdata.GetTrait(Trait.WILL);
                furtDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "FURT: " + pdata.GetTrait(Trait.FURTIVITY);
                persptDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "PERS: " + pdata.GetTrait(Trait.PERSEPTION);
                reflxDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "REFLX: " + pdata.GetTrait(Trait.REFLEX);
                constDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "CONS: " + pdata.GetTrait(Trait.CONSTITUTION);
                domDiaplay.GetComponentInChildren<TextMeshProUGUI>().text = "DOM: " + pdata.GetTrait(Trait.DOMINANCE);
            }
    }
}
