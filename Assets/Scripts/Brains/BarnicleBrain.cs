using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarnicleBrain : Brain
{
    

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
