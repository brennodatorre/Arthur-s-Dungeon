using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarnicleBrain : Brain
{
    
    

    public override void doIntent(Entity caster, Entity[] targets)
    {
        Entity _player = RoundManager.Instance.player ;

        switch (currentIntent)
        {
            case Intent.ATTACK:

                
                actionQueue.Enqueue("EnemyAttack", () => caster.doBasicAtkCaller(_player));
                break;

            case Intent.SKILL:
                        
                if (skilllToUse.targetType == Skill.SkillTarget.Self) SkillManager.Instance.doSkill( caster, caster, skilllToUse);
                else if (skilllToUse.targetType == Skill.SkillTarget.SingleAlly) {

                    List<Entity> allies = new List<Entity>(RoundManager.Instance.enemies.Where((Entity e) => e != caster));

                    Entity targetAlly = allies[UnityEngine.Random.Range(0, allies.Count)];

                    SkillManager.Instance.doSkill( targetAlly, caster, skilllToUse);
                    
                }
                else if (skilllToUse.isOffensiveSkill) SkillManager.Instance.doSkill( _player, caster, skilllToUse);

                break;


            case Intent.NONE:
                Debug.Log(gameObject.name + " has no intent set.");
                break;
        }
    }


    public override void getIntent()
    {
        Entity barnicle = GetComponent<Entity>();

        if (RoundManager.Instance.enemies.Length > 1 //more than one enemy alive
            && barnicle.getMP() > skillNeurons[0]._skill.mpCost  // has enough MP 
            && !barnicle.hasEffect(ActiveEffectManager.Instance.statusEffectPrefabs.ShieldingWithBodyEffect) // is not already shielding
        )
        {
            currentIntent = Intent.SKILL;

            skilllToUse = getRandomSkillNeuron();
            
        }
        else 
        {
            currentIntent = Intent.ATTACK;
            
        }

        intentMessage = intentToString();
    }
}
