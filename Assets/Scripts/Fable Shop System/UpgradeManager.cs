using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    public static UpgradeManager Instance;

    public List<Upgrade> upgrades = new List<Upgrade>();


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }
    }


    public void applyUpgrade(Upgrade.UpgradeName upgradeName)
    {
        // This method can be used to apply a specific upgrade based on the type
        switch (upgradeName)
        {
            case Upgrade.UpgradeName.Entropic_Ashes:
                upgradeATK(1, 4);
                break;
            case Upgrade.UpgradeName.Coagulated_Blood:
                upgradeDEF(1);
                break;
            case Upgrade.UpgradeName.Merlins_Seed:
                upgradeMAXMP(5);
                break;
            case Upgrade.UpgradeName.Survivors_Cristal:
                upgradeMAXHP(5);
                break;
            default:
                Debug.LogWarning("No valid upgrade type provided.");
                break;
        }
    }



    public void upgradeATK(int number, int sides)
    {
        PlayerData.Instance.getBaseATK().AddDice(number, sides);    
    }
    public void upgradeDEF(int amount) {PlayerData.Instance.setDEF(amount); }
    public void upgradeMAXMP(int amount) { PlayerData.Instance.setMaxMP(amount); }
    public void upgradeMAXHP(int amount) { PlayerData.Instance.setMaxHP(amount); }
}
