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


    /// <summary>
    /// Returns a random skill that is not in the excludeSkills list. If all skills are in the excludeSkills list, returns null.
    /// </summary>
    /// <returns> An instance of a random skill</returns>
    public Skill getRandom(List<Skill> excludeSkills)
    {
        List<Skill> availableSkills = new List<Skill>();

        foreach (Skill skill in skills)
        {
            bool hasSkill = false;

            foreach (Skill sk in skills)
            {
                if (sk.skillName == skill.skillName)
                {
                    hasSkill = true;
                    break; 
                }
            }

            if (!hasSkill) availableSkills.Add(skill);
            
        }

        if (availableSkills.Count == 0) return null;

        int index = Random.Range(0, availableSkills.Count);
        return Instantiate(availableSkills[index]);
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
