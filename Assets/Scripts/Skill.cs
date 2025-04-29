using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "RPG/Skill")]
public class Skill : ScriptableObject
{
    public enum SkillOrigin { Roses, Hex, Landreas, Arthur, System, Unknown };
    public enum SkillTarget { Single, Multi, Self };


    public string skillName;
    public int mpCost;
    public int cooldown;
    public int currentCooldown;
    public string description;

    public AudioClip soundEffect;
    public SkillOrigin origin;
    public SkillTarget targetType;
    public bool isSupportAction;
    public bool isStackable;

    public int limitPerTurn = 999; // The number of times this skill can be used per turn
    public int currentUsesPerTurn = 0; // The number of times this skill has been used in the current turn

    

    // Add any other properties or methods you need for the skill

    public Skill(string skillName, int mpCost, int cooldown, string description, AudioClip soundEffect, SkillOrigin origin, SkillTarget targetType)
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
        if ( 
            currentCooldown == 0 &&
            currentUsesPerTurn < limitPerTurn &&
            caster.getMP() >= mpCost && // Check if the caster has enough MP
            (isSupportAction ? caster.hasSupAction : true) &&// Check if the caster has a support action available
            checkIfStackCanBeApplied(target) // Check if the skill can be applied to the target
        ) 
        {
            currentUsesPerTurn++; // Increment the uses per turn
            return true;
        } 
        else {
            return false; // Return false if any of the conditions are not met
        }

        
    }

    private bool checkIfStackCanBeApplied(Entity target) {
        if (target.hasEffect(this)) // Check if the target has the effect of this skill
        {
            if (isStackable) // Check if the skill is stackable
            {
                return true; // Return true if the skill is stackable
            } 
            else 
            {
                return false; // Return false if the skill is not stackable
            }
        } 
        else 
        {
            return true; // Return true if the target does not have the effect of this skill
        } 
    
    }

}
