using System;

using System.Collections.Generic;

using UnityEngine;


public class Brain : MonoBehaviour
{
    [System.Serializable]
    public class skillNeuron 
    {
        public Skill _skill;
        [Range (0,100)] public int weight;

        
    }

    public enum Intent { ATTACK, SKILL, NONE };

    private ActionQueue actionQueue;


    [Range (0,100)] public int basicAtkChance = 70;

    [SerializeField] [Tooltip ("Auto normalize with ContextMenu")]
    public List<skillNeuron> skillNeurons = new List<skillNeuron>();

    public Intent currentIntent = Intent.NONE;
    public string intentMessage = "";
    public Skill skilllToUse;


    void Start()
    {
        actionQueue = RoundManager.Instance.actionQueue;
    }

    public void getIntent()
    {
        int rand = UnityEngine.Random.Range (0,100);

        if (rand < basicAtkChance) // will ATK
        {
            currentIntent = Intent.ATTACK;

            Debug.Log(gameObject.name + " will use basic attack.");
        }
        else // will use skill
        {
            currentIntent = Intent.SKILL;

            skilllToUse = getRandomSkillNeuron();

            if (skilllToUse == null)
            {
                currentIntent = Intent.ATTACK;
                Debug.LogWarning(gameObject.name + " tried to use a skill but has no skills assigned, will use basic attack instead.");
                return;
            }

            Debug.Log(gameObject.name + " will use skill: " + skilllToUse.skillName);
            
        }

        intentMessage = intentToString();
    }

    public void doIntent(Entity caster, Entity[] targets)
    {
        Entity target = targets[UnityEngine.Random.Range(0, targets.Length)];

        switch (currentIntent)
        {
            case Intent.ATTACK:
                
                actionQueue.Enqueue("EnemyAttack", () => caster.doBasicAtkCaller(target));
                break;

            case Intent.SKILL:
                        
                if (skilllToUse.targetType == Skill.SkillTarget.Self) target = caster;
                
                if (skilllToUse.isOffensiveSkill) SkillManager.Instance.doSkill( target, caster, skilllToUse);
                else {SkillManager.Instance.doSkill( caster, caster, skilllToUse);}
            
                
                break;

            case Intent.NONE:
                Debug.Log(gameObject.name + " has no intent set.");
                break;
        }
    }


    public void clearIntent()
    {
        currentIntent = Intent.NONE;
        skilllToUse = null;
    }

    private Skill getRandomSkillNeuron()
    {
        int rand = UnityEngine.Random.Range(0, 100);
        int cumulative = 0;

        wheighSkillChance();

        foreach (var neuron in skillNeurons)
        {
            cumulative += neuron.weight;
            if (rand < cumulative)
            {
                return neuron._skill;
            }
        }

        return null; //fallback, should not happen
    }

    public string intentToString()
    {
        switch (currentIntent)
        {
            case Intent.ATTACK:
                return GetComponent<Entity>().baseATK.ToString();

            case Intent.SKILL:
                if (skilllToUse.mainDice == null) return "???";
                return skilllToUse.mainDice.ToString();

            default:
                return "No Intent";   
        }         
    }

    // wheight the chance of using each skill in inspector
    [ContextMenu("Normalize Weights")]
    private void wheighSkillChance()
    {
        if (skillNeurons == null || skillNeurons.Count == 0) return;

        int totalWeight = 0;

        foreach (var skill in skillNeurons) totalWeight += skill.weight;

        if (totalWeight == 100 || totalWeight <= 0) { return; }

        float unitWorth = 100f / totalWeight;


        int newTotal = 0;

        for (int i = 0; i < skillNeurons.Count; i++)
        {
            // handles rounding error
            if (i == skillNeurons.Count - 1)
            {
                skillNeurons[i].weight = 100 - newTotal;
                break;
            }

            skillNeurons[i].weight = Mathf.RoundToInt(skillNeurons[i].weight * unitWorth);
            newTotal += skillNeurons[i].weight;
        } 
    }














}
