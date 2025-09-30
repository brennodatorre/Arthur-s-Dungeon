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
    [TextArea(3, 10)]
    public string description;
    public int duration;
    public int currentDuration = 0;

    public TurnPhaseOfEffect turnPhaseOfEffect;
    public StatusEffectType effectType;

    public Skill effectFromSkill; //the skill that applied this effect

    public Action effectAct;
    public Action endEffectAct;

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
    


}
