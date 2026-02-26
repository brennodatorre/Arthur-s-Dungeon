using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item")]
public class Item : ScriptableObject
{

    public enum ItemActionType { Main, Sup, Bonus };
    public enum ItemTarget { Single, Multi, Self };

    public string itemName;
    public string description;
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

}
