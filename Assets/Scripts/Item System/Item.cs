using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Properties;


[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item")]
public class Item : ScriptableObject 
{

    
    

    public string itemName;
    public string itemID;
    [TextArea(3, 10)]
    public string description;
    [TextArea(5, 15)] [SerializeField] 
    private string fullDescription;
    public bool hasBeenUsed = false;
    public bool isUsableInBattle = true;

    public DiceRoll mainDiceRoll = new DiceRoll();
    public float extraInput;

    public Sprite sprite;
    public int value;
    public Origin itemOrigin;
    public ActionType actionType;
    public Target targetType;
    public bool isPAHTItem;
    public AudioClip soundEffect;
    public StatusEffect statusEffect;


    [Space(10)]
    [SerializeField]List<Property> properties = new List<Property>();



    public Item() { }
    public Item(string _itemName, ActionType _itemActionType)
    {
        itemName = _itemName;
        actionType = _itemActionType;
    }

    public Item Clone()
    {
        return Instantiate(this);
    }

    public bool CanBeUsed(Entity caster, Entity target, Item item)
    {
        bool canUse = true;

       
        if (actionType == ActionType.Sup && caster.currentSupActions <= 0)
        {
            canUse = false;
            LogManager.Instance.AddLog( "No SUPPORT actions left to use " + itemName );
        }
        else if (actionType == ActionType.Main && caster.currentMainActions <= 0)
        {
            canUse = false;
            LogManager.Instance.AddLog( "No MAIN actions left to use " + itemName );
        }
        else if (statusEffect != null && !statusEffect.checkIfStackCanBeApplied(target))
        {
            canUse = false;
            LogManager.Instance.AddLog( "Cannot apply " + statusEffect.effectName + " to " + target.entityName + " right now." );
        }

        return canUse;

        
    }


    public bool HasProperty(Property property)
    {
        return properties.Contains(property);
    }


    public string GetFullDescription()
    {
        if (string.IsNullOrEmpty(fullDescription))
        {
            return description;
        }
        else if (hasBeenUsed)
        {
            return fullDescription;
        }
        else
        {
            return "???";
        }
    }

}

