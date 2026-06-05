using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Properties;

[CreateAssetMenu(fileName = "NewSkill", menuName = "RPG/Skill")]
public class Skill : ScriptableObject
{



    public string skillName;
    public string skillID; 
    public int mpCost;
    public int cooldown;
    public int currentCooldown;
    [TextArea(3, 10)]
    public string description;
    [TextArea(5, 15)] [SerializeField] 
    private string fullDescription;
    public bool hasBeenUsed = false;
    public int fableCost;
    public AudioClip soundEffect;

    public DiceRoll mainDice = new DiceRoll();
    public float extraInput;
    

    [Space(10)]
    public Origin origin;
    public Target targetType;
    public ActionType actionType = ActionType.Sup;
    public bool isOffensiveSkill = false;
    [Tooltip ("if the skill needs to be pressed and held totally in order to activate")]
    public bool isPAHTSkill = false; //press and hold target
    

    [Space(10)]
    public bool isStackable;
    [Tooltip ("The number of times this skill can be used per turn")]
    public int limitPerTurn = 999; 
    [Tooltip("The number of times this skill has been used in the current turn")]
    public int currentUsesPerTurn = 0;


    [Space(10)]
    public StatusEffect statusEffect;

    


    public Skill(string skillName, int mpCost, int cooldown, string description, AudioClip soundEffect, Origin origin, Target targetType)
    {
        this.skillName = skillName;
        this.mpCost = mpCost;
        this.cooldown = cooldown;
        this.currentCooldown = 0;
        this.description = description;
        this.soundEffect = soundEffect;
        this.origin = origin;
        this.targetType = targetType;
    }

    public Skill() { } // Default constructor for Unity serialization


    public Skill Clone()
    {
        return Instantiate(this);
    }

    public void ResetCooldown()
    {
        currentCooldown = 0; // Reset the cooldown to 0
    }

    public void ResetUsesPerTurn()
    {
        currentUsesPerTurn = 0; // Reset the uses per turn to 0
    }

    public bool CanBeUsed(Entity caster, Entity target)
    {

        bool canUse = true;

        if (currentCooldown > 0 )
        {
            canUse = false;
            LogManager.Instance.AddLog( skillName + " is on COOLDOWN");
        }
        else if (currentUsesPerTurn >= limitPerTurn)
        {
            canUse = false;
            LogManager.Instance.AddLog( skillName + " has reached its USAGE LIMIT for this turn");
        }
        else if (caster.getMP() < mpCost)
        {
            canUse = false;
            LogManager.Instance.AddLog( "Not enough MP to use " + skillName );
        } 
        else if (actionType == ActionType.Sup && caster.currentSupActions <= 0)
        {
            canUse = false;
            LogManager.Instance.AddLog( "No SUPPORT actions left to use " + skillName );
        }
        else if (actionType == ActionType.Main && caster.currentMainActions <= 0)
        {
            canUse = false;
            LogManager.Instance.AddLog( "No MAIN actions left to use " + skillName );
        }
        else if (statusEffect != null && !statusEffect.checkIfStackCanBeApplied(target))
        {
            canUse = false;
            LogManager.Instance.AddLog( "Cannot apply " + statusEffect.effectName + " to " + target.entityName + " right now." );
        }

        
         


        if (!canUse)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.skill_unable_sound);
            return false;
        }


        currentUsesPerTurn++; // Increment the uses per turn
        return true;
        
       

        
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


    /// <summary>
    /// compares this skill to another skill based on their skillID. Returns true if they are the same, false otherwise.
    /// </summary>
    public bool  CompareTo(Skill otherSkill)
    {
        if (otherSkill == null) return false;
        return this.skillID == otherSkill.skillID;
    }

   
}
