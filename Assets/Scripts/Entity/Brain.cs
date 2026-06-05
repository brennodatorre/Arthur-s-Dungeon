using System;

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;
using static Properties;


public class Brain : MonoBehaviour
{
    [System.Serializable]
    public class skillNeuron 
    {
        public Skill _skill;
        [Range (0,100)] public int weight;

        public Sprite altSprite;

        
    }

    public enum Intent { ATTACK, SKILL, NONE };

    [HideInInspector] public ActionQueue actionQueue;


    [Range (0,100)] public int basicAtkChance = 70;

    [SerializeField] [Tooltip ("Auto normalize with ContextMenu")]
    public List<skillNeuron> skillNeurons = new List<skillNeuron>();

    public Intent currentIntent = Intent.NONE;
    public string intentMessage = "";
    public Skill skilllToUse;

    public Sprite originalSprite;
    

    protected virtual void Start()
    {
        originalSprite = GetComponent<Image>().sprite;  
        actionQueue = RoundManager.Instance.actionQueue;
        foreach (var neuron in skillNeurons)
        {

            neuron._skill = neuron._skill.Clone();


            if (neuron._skill == null)
            {
                Debug.LogWarning(gameObject.name + " has a skillNeuron with no skill assigned.");
            }
            
        }
    }

    public virtual void getIntent()
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

            
            
        }

        intentMessage = intentToString();
    }

    public virtual void doIntent(Entity caster, Entity[] targets)
    {
        Entity target = targets[UnityEngine.Random.Range(0, targets.Length)];

        switch (currentIntent)
        {
            case Intent.ATTACK:
                
                actionQueue.Enqueue("EnemyAttack", () => caster.doBasicAtkCaller(target));
                break;

            case Intent.SKILL:
                        
                if (skilllToUse.targetType == Target.Self) SkillManager.Instance.doSkill( caster, caster, skilllToUse);

                else if (skilllToUse.targetType == Target.SingleAlly) {

                     List<Entity> allies = new List<Entity>(RoundManager.Instance.enemies.Where((Entity e) => e != caster));

                    Entity targetAlly = allies[UnityEngine.Random.Range(0, allies.Count)];

                    SkillManager.Instance.doSkill( targetAlly, caster, skilllToUse);
                    
                    }

                if (skilllToUse.isOffensiveSkill) SkillManager.Instance.doSkill( target, caster, skilllToUse);

                
            
                
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

    protected Skill getRandomSkillNeuron()
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
    protected void wheighSkillChance()
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



    /// <summary>
    /// Sets the alternative sprite of the neuron that has the specified skill. If no neuron has the skill, does nothing.
    /// </summary>
    public void SetAltSpriteOfNeuronWithSkill(Skill skill)
    {
        foreach (var neuron in skillNeurons)
        {
            if (neuron._skill.CompareTo(skill))
            {
                if (neuron.altSprite != null) {
                    GetComponent<Image>().sprite = neuron.altSprite;
                    GetComponent<Entity>().crackingSpriteOverlay.sprite = neuron.altSprite;
                    }
                
            }
        }

        
    }


    public void setOriginalSprite()
    {
        GetComponent<Image>().sprite = originalSprite;
        GetComponent<Entity>().crackingSpriteOverlay.sprite = originalSprite;
    }









}
