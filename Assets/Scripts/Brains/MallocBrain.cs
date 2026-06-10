using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallocBrain : Brain
{
    public AudioClip printSound;
    public GameObject bitPrefab;

    private Entity malloc;

    public override void WakeUp()
    {
       
        
        malloc = GetComponent<Entity>();

        ActiveEffectManager.Instance.addBitRateEffect(malloc, malloc);

    }

    public override void getIntent()
    {
        RoundManager rm = RoundManager.Instance;

        

        if ( CombatSetter.Instance.availableSpots.Count > 0)
        {
            currentIntent = Intent.SPECIAL;

            specialActionToUse = () => {
                AudioManager.Instance.PlaySound(printSound) ;
                CombatSetter.Instance.AddEntityToCombat(bitPrefab, true);
                };

            
        }
        else 
        {
            currentIntent = Intent.ATTACK;
            
        }

        intentMessage = intentToString();
    }
}
