using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public Skill GetSkill(string skillName)
    {
        foreach (Skill skl in skills)
        {
            if (skl.skillName == skillName)
            {
                return Instantiate(skl);
            }
        }

        Debug.LogWarning("Skill not found: " + skillName);
        return null;
    }
}
