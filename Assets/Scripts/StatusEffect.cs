using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "RPG/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public enum TurnPhaseOfEffect { Start, End, Both };

    int duration;
    int currentDuration;

    public string effectName;
    public string description;

    public enum StatusEffectType { Buff, Debuff, Neutral };
    public TurnPhaseOfEffect turnPhaseOfEffect;

    public Skill effectFromSkill; //the skill that applied this effect


    public StatusEffect(){}
    public StatusEffect( Skill effectFromSkill, string effectName = null, string description = null, int duration = 0, int currentDuration = 0, TurnPhaseOfEffect turnPhaseOfEffect = TurnPhaseOfEffect.Start)
    {
        this.effectName = effectName;
        this.description = description;
        this.duration = duration;
        this.currentDuration = currentDuration;
        this.turnPhaseOfEffect = turnPhaseOfEffect;
        this.effectFromSkill = effectFromSkill;
    }

}
