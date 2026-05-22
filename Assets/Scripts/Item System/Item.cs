using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item")]
public class Item : ScriptableObject
{

    public enum ItemActionType { Main, Sup, Bonus };
    public enum ItemTarget { Single, Multi, Self };

    public enum ItemProperty
    {
        ROCKY = 0
    }
    

    public string itemName;
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
    public Entity.EntityOrigin itemOrigin;
    public ItemActionType actionType;
    public ItemTarget targetType;
    public bool isPAHTItem;
    public AudioClip soundEffect;


    [Space(10)]
    [SerializeField]List<ItemProperty> properties = new List<ItemProperty>();



    public Item() { }
    public Item(string _itemName, ItemActionType _itemActionType)
    {
        itemName = _itemName;
        actionType = _itemActionType;
    }

    public bool CanBeUsed(Entity caster, Entity target, Item item)
    {
        bool canUse = true;

       
        if (actionType == ItemActionType.Sup && caster.currentSupActions <= 0)
        {
            canUse = false;
            LogManager.Instance.AddLog( "No SUPPORT actions left to use " + itemName );
        }
        if (actionType == ItemActionType.Main && caster.currentMainActions <= 0)
        {
            canUse = false;
            LogManager.Instance.AddLog( "No MAIN actions left to use " + itemName );
        }

        return canUse;

        
    }


    public bool HasProperty(ItemProperty property)
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

