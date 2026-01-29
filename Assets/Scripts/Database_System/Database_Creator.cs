using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Database/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items;

    /// <summary>
    /// 
    /// </summary>
    /// <returns> An instance of a random item</returns>
    public Item getRandom()
    {
        if (items.Count == 0) return null;

        int index = Random.Range(0, items.Count);
        return Instantiate(items[index]);
    }

        
    
}



[CreateAssetMenu(menuName = "Database/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    public List<Skill> skills;

    /// <summary>
    ///
    /// </summary>
    /// <returns> An instance of a random skill</returns>
    public Skill getRandom()
    {
        if (skills.Count == 0) return null;
        
        int index = Random.Range(0, skills.Count);
        return Instantiate(skills[index]);
    }
}

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
}

