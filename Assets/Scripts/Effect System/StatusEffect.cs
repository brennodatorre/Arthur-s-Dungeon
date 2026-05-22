using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "RPG/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public enum TurnPhaseOfEffect { Start, End, Both };
    public enum StatusEffectType { Buff, Debuff, Neutral };


    public Sprite sprite;
    public string effectName;
    public string statusEffectID;   

    public DiceRoll mainRoll = new DiceRoll();

    public bool isStackable;


    [TextArea(3, 10)]
    public string description;
    public int duration;
    public int currentDuration = 0;

    public AudioClip effectSound;

    public TurnPhaseOfEffect turnPhaseOfEffect;
    public StatusEffectType effectType;

    public Skill effectFromSkill; //the skill that applied this effect

    public Action effectAct;
    public Action endEffectAct;
    public Action callbackEffect;

    public Entity caster;
    public Entity target;


    public StatusEffect() { }
    public StatusEffect
    (
        Skill _effectFromSkill, string _effectName, string _description, int _duration,
        Entity _caster, Entity _target,
        TurnPhaseOfEffect _turnPhaseOfEffect, StatusEffectType _efctType,
        Action _effect, Action _endEffect
    )
    {
        this.effectName = _effectName;
        this.description = _description;
        this.duration = _duration;
        this.turnPhaseOfEffect = _turnPhaseOfEffect;
        this.effectFromSkill = _effectFromSkill;
        this.effectType = _efctType;
        this.effectAct = _effect;
        this.endEffectAct = _endEffect;
        this.target = _target;
        this.caster = _caster;
    }

    public StatusEffect Clone()
    {
        return Instantiate(this);
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
                LogManager.Instance.AddLog( "Target already has the effect " + effectName );
                return false; // Return false if the skill is not stackable
            }
        } 
        else 
        {
            return true; // Return true if the target does not have the effect of this skill
        } 
    
    }
    


}
