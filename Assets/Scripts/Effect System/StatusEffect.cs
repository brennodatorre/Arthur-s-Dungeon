using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "RPG/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public enum TurnPhaseOfEffect { Start, End, Both };
    public enum StatusEffectType { Buff, Debuff, Neutral };
    //overrides an action
    public enum OverideEffectType { NONE, TAKE_DAMAGE, HEAL, BLOCK, DAMAGE, GAIN_DEF, TARGETING };
    // block/tag effect options
    public enum BlockerType { NONE, 
    TAKING_DAMAGE, HEALING, 
    INCREASING_DAMAGE, DECREASING_DAMAGE, 
    STUNNING, 
    MP_DRAINING, MP_GAINING, 
    ACTION_GAINING, ACTION_LOSING, 
    GAIN_ATK_ADVANTAGE, LOSE_ATK_ADVANTAGE ,
    GAIN_DEF, LOSE_DEF,
    ATK_MOD_INCREASE, ATK_MOD_DECREASE,
    TARGETING
    };


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

    public Action effectAct;
    public Action endEffectAct;
    public Action callbackEffect;

    public Action<object []> overideEffectAct;
    public OverideEffectType overideEffectType;

    [Tooltip ("List of blocker types that this effect blocks")]
    public List<BlockerType> blocks = new();
    [Tooltip ("List of tags that this effect applies")]
    public List<BlockerType> tags = new();
    



    [Space(20)] [Header ("RUNTIME DEGUG -----------------------")]

    public Entity caster;
    public Entity target;

    public Skill appliedBySkill;
    public StatusEffect statusEffectFrom;
    public GameObject iconDisplay;
    public bool isBeingRemoved = false;


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
        this.appliedBySkill = _effectFromSkill;
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

    public bool checkIfStackCanBeApplied(Entity target) {
        
        foreach (StatusEffect effect in target.activeStatusEffects)
        {
            foreach (BlockerType blocker in effect.blocks)
            {
                if (this.tags.Contains(blocker))
                    return false;
            }
        }

        if (target.hasEffect(this))
        {
            if (isStackable) return true;

            LogManager.Instance.AddLog("Target already has the effect " + effectName);
            return false;
        }

        return true;
    
    }


  
    


}
