
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePage : MonoBehaviour
{

    public bool selected;
    private Color pageColor; // Default color for unselected pages\
    public int upgradeType; // Type of upgrade this page provides
    public int upgradeFableCost;
    public string upgradeDescription;

    private void Awake()
    {
        pageColor = GetComponentInParent<Image>().color; // Store the default color of the page
    }



    public void clickPage(){
        selected = !selected;
        FindObjectOfType<AudioManager>().PlaySkillPageSelectdSound();
        if (selected) { GetComponentInParent<Image>().color = Color.magenta; }
        else { GetComponentInParent<Image>().color = pageColor; }
    }


    public void setRandomPage()
    {
        // This method can be used to get a random upgrade for the player
        int randomUpgrade = Random.Range(0, 4); // Randomly choose an upgrade type
        this.upgradeType = randomUpgrade; // Set the upgrade type for this page

        switch (randomUpgrade)
        {
            case 0:
                this.upgradeFableCost = 3; 
                this.upgradeDescription = "Cost: " + upgradeFableCost + "\n" + "Increases the player's attack power by 1d4.";
                break;
            case 1:
                this.upgradeFableCost = 2; 
                this.upgradeDescription = "Cost: " + upgradeFableCost + "\n" + "Increases the player's defense by 1.";
                break;
            case 2:
                this.upgradeFableCost = 2; 
                this.upgradeDescription = "Cost: " + upgradeFableCost + "\n" + "Increases the player's maximum mana points by 5.";
                break;
            case 3:
                this.upgradeFableCost = 2; 
                this.upgradeDescription = "Cost: " + upgradeFableCost + "\n" + "Increases the player's maximum health points.";
                break;
            default:
                Debug.LogWarning("No valid upgrade type provided.");
                break;
        }
        
    }


    public void applyUpgrade(int upgradeType)
    {
        // This method can be used to apply a specific upgrade based on the type
        switch (upgradeType)
        {
            case 0:
                upgradeATK();
                break;
            case 1:
                upgradeDEF(1); // Example value, can be changed
                break;
            case 2:
                upgradeMAXMP(5); // Example value, can be changed
                break;
            case 3:
                upgradeMAXHP(5); // Example value, can be changed
                break;
            default:
                Debug.LogWarning("No valid upgrade type provided.");
                break;
        }
    }

    

    public void upgradeATK() { }
    public void upgradeDEF(int amount) {PlayerData.Instance.actualDef += amount; }
    public void upgradeMAXMP(int amount) { PlayerData.Instance.actualMaxMP += amount; }
    public void upgradeMAXHP(int amount) { PlayerData.Instance.actualMaxHP += amount; }

}
