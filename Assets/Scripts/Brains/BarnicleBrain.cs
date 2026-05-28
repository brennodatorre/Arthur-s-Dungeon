using System.Collections;
using System.Collections.Generic;
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

                    Entity targetAlly = RoundManager.Instance.enemies[UnityEngine.Random.Range(0, RoundManager.Instance.enemies.Length)].GetComponent<Entity>();

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
        

        if (RoundManager.Instance.enemies.Length > 1 && GetComponent<Entity>().getMP() > skillNeurons[0]._skill.mpCost )
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
