
using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "RPG/Upgrade")]
public class Upgrade : ScriptableObject
{
    public enum UpgradeName
    {
        Survivors_Cristal,
        Coagulated_Blood,
        Entropic_Ashes,
        Merlins_Seed
    }

    public UpgradeName upgradeName;
    public int upgradeFableCost;
    [TextArea(3, 10)]
    public string upgradeDescription;

    public Sprite image;
    public Vector3 imageOffeset;


    public Upgrade(UpgradeName _upName, int _upFableCost, string _upDescript)
    {
        this.upgradeName = _upName;
        this.upgradeFableCost = _upFableCost;
        this.upgradeDescription = _upDescript;
    }
    public Upgrade() { }

}
