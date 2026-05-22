using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Status Effect Database")]
public class StatusEffectDatabase : ScriptableObject
{
    public List<StatusEffect> StatusEffects;


    /// <summary>
    ///
    /// </summary>
    /// <returns> An instance of a random status effect</returns>
    public StatusEffect getRandom()
    {
        if (StatusEffects.Count == 0) return null;
        
        int index = Random.Range(0, StatusEffects.Count);
        return Instantiate(StatusEffects[index]);
    }


    public StatusEffect GetStatusEffectByID(string statusEffectID)
    {
        foreach (StatusEffect effect in StatusEffects)
        {
            if (effect.statusEffectID == statusEffectID)
            {
                return Instantiate(effect);
            }
        }
        return null;
    }
}
