using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitBrain : Brain
{

    public StatusEffect bitRateStatusEffect;

    private Entity bit;

    protected override void Start()
    {
       base.Start(); // does base brain start 
        
        bit = GetComponent<Entity>();

        ActiveEffectManager.Instance.addBitRateEffect(bit, bit);

    }

    public override void getIntent()
    {
        

        if (skillNeurons.Count > 0
            && bit.getMP() > skillNeurons[0]._skill.mpCost  // has enough MP   
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
