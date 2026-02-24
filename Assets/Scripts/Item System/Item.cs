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
        if (
            //currentCooldown == 0 &&
            //currentUsesPerTurn < limitPerTurn &&
            //caster.getMP() >= mpCost && // Check if the caster has enough MP

            // Check if the caster has a support or main action available
            ((actionType == ItemActionType.Sup ? caster.currentSupActions > 0 : false) ||
            (actionType == ItemActionType.Main ? caster.currentMainActions > 0 : false) ||
            actionType == ItemActionType.Bonus ? true : false)
            //&& checkIfStackCanBeApplied(target, skl) // Check if the skill can be applied to the target
        ) 
        {
            //currentUsesPerTurn++; // Increment the uses per turn
            return true;
        } 
        else {
            return false; // Return false if any of the conditions are not met
        }

        
    }

}
