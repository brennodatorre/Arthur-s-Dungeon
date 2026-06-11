using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CinzentadoBrain : Brain
{
   public GameObject rootsPrefab;
   public AudioClip rootSpawnSoundEffect;

   private Entity cinzentado;
   private List<Entity> roots = new List<Entity>();

   public override void WakeUp()
    {
       
        
        cinzentado = GetComponent<Entity>();

        

    }


    public override void getIntent()
    {
        RoundManager rm = RoundManager.Instance;

        int rand = UnityEngine.Random.Range (0,100);

        
        // create roots if there is space and basic atk was not triggered
        if ( CombatSetter.Instance.availableSpots.Count > 0 && (rand > basicAtkChance) )
        {
            currentIntent = Intent.SPECIAL;

            specialActionToUse = () => {
                AudioManager.Instance.PlaySound(rootSpawnSoundEffect) ;

                if (roots.Count == 0) ActiveEffectManager.Instance.addUnreachableEffect(cinzentado, cinzentado);
                CombatSetter.Instance.AddEntityToCombat(rootsPrefab);

                roots.Clear();
                roots = rm.enemies.Where(r => r.entityID == rootsPrefab.GetComponent<Entity>().entityID).ToList();
                
                };

            
        }
        else 
        {
            currentIntent = Intent.ATTACK;
            
        }

        intentMessage = intentToString();
    }



    public void RemoveRoot(Entity _root) 
    {
        roots.Remove(_root); 
        if (roots.Count <1)
        {
            cinzentado.removeEffect(ActiveEffectManager.Instance.statusEffectPrefabs.UnreachbleEffct.statusEffectID);
        }
    }

}
