using System;

using System.Collections.Generic;


using UnityEngine;

public class ActiveEffectManager : MonoBehaviour
{

    public RoundManager roundManager;
    public LogManager logManager;
    public AudioManager audioManager;

    [SerializeField] public List<effectNode> activeEffects = new List<effectNode>();
    [SerializeField] public List<effectNode> endEffects = new List<effectNode>(); //list of actions to do at the end of the turn
    [SerializeField] public List<effectNode> startEffects = new List<effectNode>(); //list of actions to do at the end of the turn

    private RoundManager.TurnPhase currentPhase; //the current phase of the turn

    




    public void Update() {
        // if turn phase changed and there are active effects, execute
        if (currentPhase != roundManager.currentPhase && activeEffects.Count > 0) {

           currentPhase = roundManager.currentPhase; //update the current phase
           

           
            
            if (currentPhase == RoundManager.TurnPhase.Action)
            {
                //imlpoement the action phase here
            }

            else if (currentPhase == RoundManager.TurnPhase.End)
            {
                
                foreach (effectNode effect in endEffects)
                {
                    if (roundManager.currentTurn != effect.target){continue;} //skip the effect if the target is not the current turn
                    if (effect.turns == 1) { effect.endEffect.Invoke(); } //execute end effect if it exists
                    effect.roundEffect.Invoke(); //execute the action
                    effect.turns--; //decrease the turns left for the effect

                }



            }
            else if (currentPhase == RoundManager.TurnPhase.Start)
            {

                foreach (effectNode effect in startEffects)
                {
                    if (roundManager.currentTurn != effect.target){continue;} //skip the effect if the target is not the current turn
                    if (effect.turns == 1) { effect.endEffect.Invoke(); } //execute end effect if it exists
                    effect.roundEffect.Invoke(); //execute the action
                    effect.turns--; //decrease the turns left for the effect

                }

            }


            RemoveDeadEffects(); //remove any dead effects from the lists

            


        }

    }
    

    public void AddEffect(Skill skill, int turns, RoundManager.TurnPhase phase , Entity target , Entity caster, Action triggerEffect, Action triggerEndEffect ) {
        
        effectNode effect = new effectNode(turns, triggerEffect, phase, triggerEndEffect, target, caster); //create a new node for the effect

        activeEffects.Add(effect); //add the effect to the list of active effects

        if (phase == RoundManager.TurnPhase.Start) {
            startEffects.Add(effect); //add the effect to the list of start effects
        } else if (phase == RoundManager.TurnPhase.End) {
            endEffects.Add(effect); //add the effect to the list of end effects
        } else {
            Debug.Log("Error: Invalid phase for effect: " + phase); //log an error message if the phase is invalid
        }
        
        
    }


    private void RemoveDeadEffects() {

        List<effectNode> effectsToRemove = new List<effectNode>(); //create a list of effects to remove

        foreach (effectNode effect in activeEffects) {
            if (effect.turns < 1) {
                effectsToRemove.Add(effect);
            } 
                
        }

        foreach (effectNode effect in effectsToRemove) {
        
            activeEffects.Remove(effect); //remove the effect from the list of active effects
            endEffects.Remove(effect); //remove the effect from the list of end effects
            startEffects.Remove(effect); //remove the effect from the list of start effects
        
                
        }
        
    }

    
}

[System.Serializable]
public class effectNode
{
    public int turns;
    public Action roundEffect;
    RoundManager.TurnPhase phase;
    public Action endEffect;
    public Entity target;
    public Entity caster;

    public effectNode(int turns, Action roundEffect, RoundManager.TurnPhase phase, Action endEffect, Entity target, Entity caster)
    {
        this.target = target;
        this.caster = caster;
    
        this.turns = turns;
        this.roundEffect = roundEffect;
        this.phase = phase;
        this.endEffect = endEffect;
    }
}

