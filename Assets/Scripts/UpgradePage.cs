

using UnityEngine;
using UnityEngine.UI;

public class UpgradePage : MonoBehaviour
{


    public Upgrade upgrade;
    public bool selected;
    private Color pageColor; // Default color for unselected pages\

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


    // public void setRandomPage()
    // {
    //     // This method can be used to get a random upgrade for the player
    //     int randomUpgrade = Random.Range(0, 4); // Randomly choose an upgrade type
        

    //     switch (randomUpgrade)
    //     {
    //         case 0: // Entropic Ashes
    //             this.upgradeName = UpgradeName.Entropic_Ashes;
    //             this.upgradeFableCost = 3; 
    //             this.upgradeDescription = "Cost: " + upgradeFableCost + "\n" + "Increases the player's attack power by 1d4.";
    //             break;
    //         case 1: // Coagulated Blood
    //             this.upgradeName = UpgradeName.Coagulated_Blood;
    //             this.upgradeFableCost = 2; 
    //             this.upgradeDescription = "Cost: " + upgradeFableCost + "\n" + "Increases the player's defense by 1.";
    //             break;
    //         case 2: // Merlins Seed
    //             this.upgradeName = UpgradeName.Merlins_Seed;
    //             this.upgradeFableCost = 2; 
    //             this.upgradeDescription = "Cost: " + upgradeFableCost + "\n" + "Increases the player's maximum mana points by 5.";
    //             break;
    //         case 3: // Survivors Cristal
    //             this.upgradeName = UpgradeName.Survivors_Cristal;
    //             this.upgradeFableCost = 2; 
    //             this.upgradeDescription = "Cost: " + upgradeFableCost + "\n" + "Increases the player's maximum health points.";
    //             break;
    //         default:
    //             Debug.LogWarning("No valid upgrade type provided.");
    //             break;
    //     }
        
    // }


    // public void applyUpgrade(UpgradeName upgradeName)
    // {
    //     // This method can be used to apply a specific upgrade based on the type
    //     switch (upgradeName)
    //     {
    //         case UpgradeName.Entropic_Ashes:
    //             upgradeATK();
    //             break;
    //         case    UpgradeName.Coagulated_Blood:
    //             upgradeDEF(1); 
    //             break;
    //         case UpgradeName.Merlins_Seed:
    //             upgradeMAXMP(5); 
    //             break;
    //         case UpgradeName.Survivors_Cristal:
    //             upgradeMAXHP(5); 
    //             break;
    //         default:
    //             Debug.LogWarning("No valid upgrade type provided.");
    //             break;
    //     }
    // }

    

    // public void upgradeATK() { }
    // public void upgradeDEF(int amount) {PlayerData.Instance.actualDef += amount; }
    // public void upgradeMAXMP(int amount) { PlayerData.Instance.actualMaxMP += amount; }
    // public void upgradeMAXHP(int amount) { PlayerData.Instance.actualMaxHP += amount; }

}
