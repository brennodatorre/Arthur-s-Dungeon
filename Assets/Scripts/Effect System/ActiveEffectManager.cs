using System;

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActiveEffectManager : MonoBehaviour
{
    public static ActiveEffectManager Instance ;


    public RoundManager roundManager;
    public LogManager logManager;
    public AudioManager audioManager;

    public StatusEffectPrefabs statusEffectPrefabs;

    //if there are too many effects, it my lag, so possible optimization can be done here
    [SerializeField] public List<StatusEffect> activeEffects = new List<StatusEffect>();


    private RoundManager.TurnPhase currentPhase; //the current phase of the turn


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }
    }



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
                    

                }

            }

            RemoveDeadEffects(); //remove any dead effects from the lists
            


            

            


        }

    }





    public void AddEffect(StatusEffect staEfct, Entity caster, Entity target, Skill appliedBySkill , Action effect, Action endEffect,  Action callbacl = null, Action <object []> overideEffect = null, StatusEffect statusEffectFrom = null)
    {
        if (target.getHP() == 0) {return; } //don't add the effect if the target is dead

        staEfct.caster = caster;
        staEfct.target = target;
        staEfct.appliedBySkill = appliedBySkill;
        staEfct.statusEffectFrom = statusEffectFrom;
        staEfct.effectAct = effect;
        staEfct.endEffectAct = endEffect;
        staEfct.callbackEffect = callbacl; //calls back to itself, i.e. Reagent Slug calling back all SE
        staEfct.overideEffectAct = overideEffect; //overide some of the actions of the target/caster

        activeEffects.Add(staEfct); //add the effect to the list of active effects  
        staEfct.iconDisplay = StatusHudManager.Instance.addStatusEffectToDisplay(staEfct);
        
    }


    private void RemoveDeadEffects() {


        foreach (StatusEffect effect in activeEffects.ToArray())
        {
            if (effect.isBeingRemoved) continue; 
            if (effect.currentDuration < 1)
            {
                effect.isBeingRemoved = true;
                
                effect.endEffectAct?.Invoke();

                effect.iconDisplay.GetComponent<StatusEffectIcon>().MarkToDie();
 
            }
        }

        activeEffects.RemoveAll(effect => effect.isBeingRemoved);
        StatusHudManager.Instance.UpdateStatusEffectDisplay();
        

        
    }

    /// <summary>
    /// Removes all Status Effects in the list, invoking their end effects
    /// </summary>
    public static void RemoveAllEffects(List<StatusEffect> effectsToRemove)
    {

        List<StatusEffect> effectsCopy = new List<StatusEffect>(effectsToRemove);

        foreach (StatusEffect effect in effectsCopy)
        {
            effect.isBeingRemoved = true;
            effect.currentDuration = 0;
            effect.endEffectAct?.Invoke();
            
            effect.iconDisplay.GetComponent<StatusEffectIcon>().MarkToDie();
            
        }

        effectsToRemove.RemoveAll(effect => true);
        StatusHudManager.Instance.UpdateStatusEffectDisplay();
        
        

        

    }

    public void KillEffect(StatusEffect effect)
    {
        if (effect == null || effect.isBeingRemoved) return;

        effect.isBeingRemoved = true;

        effect.currentDuration = 0;
        effect.endEffectAct?.Invoke();

        if (effect.iconDisplay != null)
        effect.iconDisplay.GetComponent<StatusEffectIcon>()?.MarkToDie();

        activeEffects.Remove(effect);
    }

    


   
    
    #region Effect Logic

    [System.Serializable]
    public class StatusEffectPrefabs
    {
        public StatusEffect BleedEffect;
        public StatusEffect RottingTouchEffect;
        public StatusEffect BestificationEffect;
        public StatusEffect ElectrifiedWeaponEffect;
        public StatusEffect PreparedEffect;
        public StatusEffect PlattedSoulEffect;
        public StatusEffect SlateScarEffect;
        public StatusEffect BodyShieledEffect;
        public StatusEffect ShieldingWithBodyEffect;
        public StatusEffect DefGainBlockedEffect;
        public StatusEffect BitRateEffect;
        public StatusEffect StunnedEffect;
    }


    public void addBleed(Entity target, Entity caster, Skill appliedBySkill = null, bool withSound = false)
    {
        StatusEffect inst = Instantiate(statusEffectPrefabs.BleedEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list

        AddEffect(inst, caster, target, appliedBySkill, () =>
            {
                // DiceRoll bleedDamage = new DiceRoll(); //create a new DiceRoll
                // bleedDamage.AddDice(1, 4); //
                int Bdamage = inst.mainRoll.Roll(); //roll the damage amount

                target.takeTrueDamage(Bdamage); //deal true damage to the target

                logManager.AddLog(target.entityName + " took " + Bdamage + " bleed damage. ");
            },
            () =>
            {
                target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list
            }, () => addBleed(target, caster, appliedBySkill));
    }

    public void addStunnedEffect(Entity target, Entity caster, Skill appliedBySkill = null, Item appliedByItem = null, bool withSound = false)
    {
        StatusEffect inst = Instantiate(statusEffectPrefabs.StunnedEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list

        AddEffect(inst, caster, target, appliedBySkill, () =>{},
            () =>
            {
                target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            }, () => addStunnedEffect(target, caster, appliedBySkill, appliedByItem));
    }

    public void addRottingTouch(Entity target, Entity caster, Skill appliedBySkill = null, bool withSound = false)
    {

        if (withSound)audioManager.PlaySound(statusEffectPrefabs.RottingTouchEffect.effectSound); //play skill sound   

        target.currentATK.AddDice(statusEffectPrefabs.RottingTouchEffect.mainRoll); 


        StatusEffect inst = Instantiate(statusEffectPrefabs.RottingTouchEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list of the target

        AddEffect(inst, caster, target,  appliedBySkill, () => { }, () =>
        {

            target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            target.currentATK.RemoveDice(statusEffectPrefabs.RottingTouchEffect.mainRoll); //remove 1d4 from the attack amount


        }, () => addRottingTouch(target , caster, appliedBySkill));
    }

    public void addBestified(Entity target, Entity caster , Skill appliedBySkill = null, bool withSound = false)
    {
        //add 1d6 + 2 to the attack of the user
        target.currentATK.AddDice(statusEffectPrefabs.BestificationEffect.mainRoll);
        target.currentATK.AddModifier(statusEffectPrefabs.BestificationEffect.mainRoll.modifier);

        if (withSound)
        {
            audioManager.PlaySound(statusEffectPrefabs.BestificationEffect.effectSound); //play skill sound
        }

        StatusEffect inst = Instantiate(statusEffectPrefabs.BestificationEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list

        AddEffect(inst, caster, target, appliedBySkill,() => { }, () =>
        {

            target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            target.currentATK.RemoveDice(1, 6); //remove 1d4 from the attack amount

            target.currentATK.AddModifier(-2); //remove the modifier

            DiceRoll damageAmount = new DiceRoll(); //create a new DiceRoll
            damageAmount.AddDice(1, 8); //
            int damage = damageAmount.Roll(); //roll the damage amount

            target.takeTrueDamage(damage); //deal true damage to the target

            logManager.AddLog(target.entityName + " takes " + damage + " damage from Beastial Adrenaline.");




        }, () => addBestified(target, caster ,appliedBySkill));


    }  

    public void addElectrifiedWeapon(Entity target, Entity caster, Skill appliedBySkill = null, bool withSound = false)
    {
        if (withSound)
        {
            audioManager.PlaySound(statusEffectPrefabs.ElectrifiedWeaponEffect.effectSound); //play skill sound
        }

        target.currentATK.AddModifier(statusEffectPrefabs.ElectrifiedWeaponEffect.mainRoll.modifier); //add 1d4 to the attack amount

       

        StatusEffect inst = Instantiate(statusEffectPrefabs.ElectrifiedWeaponEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list of the target

        AddEffect(inst, caster, target, appliedBySkill,() => { }, () =>
        {

            target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            target.currentATK.AddModifier(-3); //remove 1d4 from the attack amount


        }, () => addElectrifiedWeapon(target, caster, appliedBySkill));
    }

    public void addPrepared(Entity target, Entity caster, Skill appliedBySkill  = null, bool withSound = false)
    {


        if (withSound) audioManager.PlaySound(statusEffectPrefabs.PreparedEffect.effectSound); //play skill sound

        caster.GetComponent<Brain>()?.SetAltSpriteOfNeuronWithSkill(appliedBySkill) ;

        caster.atkAdvantage++;

        logManager.AddLog(caster.entityName + " is prepared");



        //////////////////////////////////////////////////

        StatusEffect inst = Instantiate(statusEffectPrefabs.PreparedEffect);
        caster.activeStatusEffects.Add(inst); //add the skill to the active effects list of the target

        AddEffect(inst, caster, caster, appliedBySkill,() => { }, () =>
        {

            caster.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            if (caster != null) caster.GetComponent<Brain>()?.setOriginalSprite();

            caster.atkAdvantage--;


        }, () => addPrepared(caster, caster, appliedBySkill));
    }

    public void addPlattedSoul(Entity target, Entity caster, Skill appliedBySkill  = null, bool withSound = false)
    {
        
        if (withSound) audioManager.PlaySound(statusEffectPrefabs.PlattedSoulEffect.effectSound);

        

        target.changeDEF(3); //add 3 to the defense amount

  

        StatusEffect inst = Instantiate(statusEffectPrefabs.PlattedSoulEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list

        AddEffect(inst, caster, target, appliedBySkill,() => { }, () =>
        {
            target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            target.changeDEF(-3); //remove 1d4 from the attack amount

        }, () => addPlattedSoul(target, caster, appliedBySkill));


    }


    public void addSlateScar(Entity target, Entity caster, Skill appliedBySkill  = null, bool withSound = false)
    {
        if (withSound) audioManager.PlaySound(statusEffectPrefabs.SlateScarEffect.effectSound);

        target.changeDEF(-1); 

        StatusEffect inst = Instantiate(statusEffectPrefabs.SlateScarEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list

        AddEffect(inst, caster, target, appliedBySkill,() => { }, () =>
        {
            target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            target.changeDEF(1); 

        }, () => addSlateScar(target, caster, appliedBySkill));

    }

    public void addBodyShielded(Entity target, Entity caster, Skill appliedBySkill  = null, bool withSound = false  )
    {
        if (withSound) audioManager.PlaySound(statusEffectPrefabs.BodyShieledEffect.effectSound);

        caster.GetComponent<Brain>()?.SetAltSpriteOfNeuronWithSkill(appliedBySkill) ;

        

        StatusEffect inst = Instantiate(statusEffectPrefabs.BodyShieledEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list

        StatusEffect shielderEffect = addShieldingWithBody( caster, target, inst, appliedBySkill, withSound);

        AddEffect(inst, caster, target, appliedBySkill, () => { }, () =>
        {
            target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list
            if (shielderEffect != null) {
                KillEffect(shielderEffect);
            }
            if (caster != null) caster.GetComponent<Brain>()?.setOriginalSprite();

        }, 
        () => addBodyShielded(target, caster, appliedBySkill),
        (object [] args) => {
            Entity attacker = (Entity)args[0];
            Entity attackTarget = (Entity)args[1];

            // redirects attack to the caster, and then calls the basic attack function of the attacker on the caster
            RoundManager.Instance.actionQueue.Enqueue("EnemyAttack", () => attacker.doBasicAtkCaller(caster, true));
            
            logManager.AddLog(caster.entityName + " blocked the ATK with its Body as a Shield.");
        }
        );

    }

    public StatusEffect addShieldingWithBody(Entity target, Entity caster, StatusEffect statusEffectFrom, Skill appliedBySkill  = null, bool withSound = false)
    {
        if (withSound) audioManager.PlaySound(statusEffectPrefabs.ShieldingWithBodyEffect.effectSound);

        StatusEffect inst = Instantiate(statusEffectPrefabs.ShieldingWithBodyEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list
        inst.duration = statusEffectFrom.duration; //set the duration of the shielding effect to be the same as the body shield effect
        inst.currentDuration = statusEffectFrom.currentDuration; //set the current duration of the shielding effect to be the same as the body shield effect

        AddEffect(inst, caster, target, appliedBySkill,() => { }
        ,() =>
        {
            target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            if (statusEffectFrom != null) {
             
                KillEffect(statusEffectFrom);
            }


        }, () => addShieldingWithBody(target, caster, statusEffectFrom, appliedBySkill));

        return inst;

    }

    public void addWallSolvent(Entity target, Entity caster, Skill appliedBySkill  = null, bool withSound = false, Item appliedByItem = null)
    {
        if (withSound) audioManager.PlaySound(statusEffectPrefabs.DefGainBlockedEffect.effectSound);

        int removedDEF = target.getDEF(); //store the previous defense value of the target

        target.changeDEF(-target.getDEF()); //set the defense to 0

        StatusEffect inst = Instantiate(statusEffectPrefabs.DefGainBlockedEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list

        if (appliedByItem != null) {
            inst.duration = (int)appliedByItem.extraInput; //set the duration of the effect to be the same as the skill's duration
            inst.currentDuration = (int)appliedByItem.extraInput; //set the current duration of the effect to be the same as the skill's duration
        }
        else if (appliedBySkill != null) {
            inst.duration = appliedBySkill.extraInput > 0 ? (int)appliedBySkill.extraInput : 3; //set the duration of the effect to be the same as the skill's duration, or 3 if the skill's duration is not set
            inst.currentDuration = appliedBySkill.extraInput > 0 ? (int)appliedBySkill.extraInput : 3; //set the current duration of the effect to be the same as the skill's duration, or 3 if the skill's duration is not set
        }


        AddEffect(inst, caster, target, appliedBySkill,() => { }, () =>
        {
            target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            target.changeDEF(removedDEF); //restore the defense value of the target

        }, () => addWallSolvent(target, caster, appliedBySkill, false, appliedByItem), 
        (object [] args) => {
            

            logManager.AddLog(target.entityName + " can no longer increase its defenses.");

        }
        
        );

    }

    public void addBitRateEffect(Entity target, Entity caster, Skill appliedBySkill  = null, bool withSound = false  )
    {
        if (withSound) audioManager.PlaySound(statusEffectPrefabs.BitRateEffect.effectSound);

        List<Entity> bits = new List<Entity>(RoundManager.Instance.enemies.Where(e => e.properties.Contains(Properties.Property.BIT))); //get all bits 
        
        bits.Remove(caster);

        
        foreach (Entity bit in bits) {
            bit.currentATK.AddModifier(1); //add 1 to the attack of all other bits
            caster.currentATK.AddModifier(1);Debug.Log(bit);
        }

        

        StatusEffect inst = Instantiate(statusEffectPrefabs.BitRateEffect);
        target.activeStatusEffects.Add(inst); //add the skill to the active effects list

        

        AddEffect(inst, caster, target, appliedBySkill, () => { }, () =>
        {
            target.activeStatusEffects.Remove(inst); //remove the effect from the active effects list

            List<Entity> bitsToRemove = new List<Entity>(RoundManager.Instance.enemies.Where((Entity e) => e.properties.Contains(Properties.Property.BIT)));
            bits.Remove(caster);
            foreach (Entity bit in bitsToRemove) {
                bit.currentATK.AddModifier(-1); //remove 1 from the attack of all other bits
            }

        }
        );

    }




    #endregion








}



