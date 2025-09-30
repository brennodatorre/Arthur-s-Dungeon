using System;

using System.Collections.Generic;


using UnityEngine;

public class ActiveEffectManager : MonoBehaviour
{

    public RoundManager roundManager;
    public LogManager logManager;
    public AudioManager audioManager;

    //if there are too many effects, it my lag, so possible optimization can be done here
    [SerializeField] public List<StatusEffect> activeEffects = new List<StatusEffect>();


    private RoundManager.TurnPhase currentPhase; //the current phase of the turn

    




    public void Update() {
        // if turn phase changed and there are active effects, execute
        if (currentPhase != roundManager.currentPhase && activeEffects.Count > 0) {

           currentPhase = roundManager.currentPhase; //update the current phase
           

            if (currentPhase == RoundManager.TurnPhase.End)
            {

                foreach (StatusEffect effect in activeEffects)
                {
                    if (roundManager.currentTurn != effect.target) { continue; } //skip the effect if the target is not the current turn
                    if (effect.turnPhaseOfEffect != StatusEffect.TurnPhaseOfEffect.End) { continue; } //skip if the effetct is not an end effect

                    effect.effectAct.Invoke(); //execute the action
                    effect.currentDuration--; //decrease the turns left for the effect
                    
                    if (effect.currentDuration == 0) { effect.endEffectAct.Invoke(); } //execute end effect if it exists

                }



            }
            else if (currentPhase == RoundManager.TurnPhase.Start)
            {

                foreach (StatusEffect effect in activeEffects)
                {
                    if (roundManager.currentTurn != effect.target) { continue; } //skip the effect if the target is not the current turn
                    if (effect.turnPhaseOfEffect != StatusEffect.TurnPhaseOfEffect.Start) { continue; } //skip if the effetct is not an start effect
                    
                    effect.effectAct.Invoke(); //execute the action
                    effect.currentDuration--; //decrease the turns left for the effect
                    
                    if (effect.currentDuration == 0) { effect.endEffectAct.Invoke(); } //execute end effect if it exists

                }

            }


            RemoveDeadEffects(); //remove any dead effects from the lists

            


        }

    }


    public void AddEffect(StatusEffect staEfct, Entity caster, Entity target, Action effect, Action endEffect)
    {

        staEfct.caster = caster;
        staEfct.target = target;
        staEfct.effectAct = effect;
        staEfct.endEffectAct = endEffect;

        activeEffects.Add(staEfct); //add the effect to the list of active effects  
        StatusHudManager.Instance.addStatusEffectToDisplay(staEfct);
        
    }


    private void RemoveDeadEffects() {


        activeEffects.RemoveAll(effect => effect.currentDuration < 1);
        
    }

    
}



