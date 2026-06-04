using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copperWarriroBrain : Brain
{
    public override void getIntent()
    {
        Entity copper = GetComponent<Entity>();

        if (
            copper.getMP() > skillNeurons[0]._skill.mpCost  // has enough MP 
            && !copper.hasEffect(ActiveEffectManager.Instance.statusEffectPrefabs.PreparedEffect) // is not already prepared
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
