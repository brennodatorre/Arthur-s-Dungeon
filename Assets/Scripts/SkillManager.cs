using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [HideInInspector] public static SkillManager Instance;

    public RoundManager roundManager;
    public LogManager logManager;
    public AudioManager audioManager;
    public ActiveEffectManager activeEffectManager;

    public List<Skill> skills = new List<Skill>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }
    }




    public void doSkill(Entity target, Entity caster, Skill skill)
    {
        // deals with self targeting skills (casting a self target into someone else)
        if (target != caster && skill.targetType == Skill.SkillTarget.Self)
        {
            audioManager.PlaySound(audioManager.skill_unable_sound);
            logManager.AddLog(skill.skillName + " can only be used on yourself.");
            roundManager.buttonManager.closeSkillMenu(); //close the skill menu after the action is done
            return;
        }
        {

        }

        if (skill.CanBeUsed(caster, target, skill))
        {
            //use up the sup action if the skill is a support action
            if (skill.actionType == Skill.SkillActionType.Sup) { caster.currentSupActions--; }
            else if (skill.actionType == Skill.SkillActionType.Main) { caster.currentMainActions--; }

            caster.loseMP(skill.mpCost); //caster lose mp

            if (skill.skillName == "Healing Tear")
            {

                roundManager.actionQueue.Enqueue("do HealingTear", () => doHealingTear(target, caster, skill)); //add the action to the queue

            }
            else if (skill.skillName == "Rotting Touch")
            {

                roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doRottingTouch(target, caster, skill)); //add the action to the queue

            }
            else if (skill.skillName == "Plated Soul")
            {

                roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doPlatedSoul(target, caster, skill)); //add the action to the queue
            }
            else if (skill.skillName == "Devour")
            {

                roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doDevour(target, caster, skill)); //add the action to the queue
            }
            else if (skill.skillName == "Bestial Adrenaline")
            {

                roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doBestialAdrenaline(target, caster, skill)); //add the action to the queue
            }
            else if (skill.skillName == "Spinal Jaw")
            {

                roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doSpinalJaw(target, caster, skill)); //add the action to the queue
            }
            else
            {
                Debug.Log("Skill not implemented yet.");
            }

        }
        else
        {
            audioManager.PlaySound(audioManager.skill_unable_sound);
            logManager.AddLog(caster.name + " cannot use " + skill.skillName + " on " + target.name + ".");
        }

        if (caster.currentMainActions <= 0)
        {
            roundManager.buttonManager.closeSkillMenu(); //close the skill menu after the action is done
            roundManager.act_menu.SetActive(false); //close the action menu after the action is done
            //roundManager.act_menu.SetActive(false); //close the target menu after the action is done
            roundManager.EndTurn(); //end the turn if the caster has no main actions left
        }
        else
        {
            //roundManager.EnableSkillTargetingUI(true);
            roundManager.buttonManager.skillMenu.SetActive(true);
        }



    }





    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////


    public Skill getSkill(string skillName)
    {
        foreach (Skill skill in skills)
        {
            if (skill.skillName == skillName)
            {
                return skill; //return the skill if it is found
            }
        }
        Debug.Log("Skill not found: " + skillName); //if the skill is not found, log an error message
        return null; //return null if the skill is not found
    }

    private bool checkMP(Skill skill, Entity caster)
    {
        if (caster.getMP() < skill.mpCost)
        {
            logManager.AddLog(caster.name + " deoes not have enough MP to cast Healing Tear.");
            return false; //return false if the caster does not have enough MP to cast the skill
        }

        return true; //return true if the caster has enough MP to cast the skill
    }

    public void resetSkills(Entity caster)
    {
        foreach (Skill skill in caster.skillsInstance)
        {
            skill.ResetUsesPerTurn(); //reset the uses per turn for each skill
        }
    }


    private IEnumerator doHealingTear(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play the healing sound

        DiceRoll healAmount = new DiceRoll(); //create a new DiceRoll for the healing amount
        healAmount.AddDice(1, 6); //add 1d6 to the healing amount

        var heal = healAmount.Roll(); //roll the healing amount
        caster.heal(heal); //heal the target for 10 HP
        

        logManager.AddLog(caster.name + " casted Healing Tear on " + target.name + " for " + heal + " HP.");

    }

    private IEnumerator doRottingTouch(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        

        audioManager.PlaySound(skill.soundEffect); //play skill sound

        DiceRoll damageAmount = new DiceRoll(); //create a new DiceRoll
        damageAmount.AddDice(1, 4); //



        target.currentATK.AddDice(1, 4); //add 1d4 to the attack amount

        logManager.AddLog(caster.name + " casted " + skill.skillName + " on " + target.name);



        //////////////////////////////////////////////////
        
        StatusEffect inst = Instantiate(skill.statusEffect);
        target.activeSkillEffects.Add(inst); //add the skill to the active effects list of the target

        activeEffectManager.AddEffect(inst, caster, target,  () => { }, () =>
        {

            target.activeSkillEffects.Remove(inst); //remove the effect from the active effects list

            target.currentATK.RemoveDice(1, 4, caster.currentATK); //remove 1d4 from the attack amount


        });

    }


    private IEnumerator doPlatedSoul(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play skill sound





        target.def += 3; //add 3 to the defense amount

        logManager.AddLog(caster.name + " casted " + skill.skillName + " on " + target.name);



        //////////////////////////////////////////////////

        StatusEffect inst = Instantiate(skill.statusEffect);
        target.activeSkillEffects.Add(inst); //add the skill to the active effects list

        activeEffectManager.AddEffect(inst, caster, target, () => { }, () =>
        {
            target.activeSkillEffects.Remove(inst); //remove the effect from the active effects list

            target.def += -3; //remove 1d4 from the attack amount
        }


        );

    }

    private IEnumerator doDevour(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play the skill sound

        int casterRoll = 0;
        if (caster.DEXTREZA > caster.ATLETISMO) { casterRoll = caster.rollTest(Entity.Trait.DEXTREZA); } //roll a test based on the caster's DEXTREZA
        else { casterRoll = caster.rollTest(Entity.Trait.ATLETISMO); } //roll a test based on the caster's ATLETISMO

        int targetRoll = target.rollTest(Entity.Trait.REFLEXOS); //roll a test based on the target's CONSTITUICAO

        //if skill sucesseds
        if (casterRoll > targetRoll)
        {
            DiceRoll lifesteal = new DiceRoll(); //create a new DiceRoll for the lifesteal amount
            lifesteal.AddDice(1, 4); //add 1d4 to
            int lifestealAmount = lifesteal.Roll(); //roll the lifesteal amount
            caster.heal(lifestealAmount); //heal the caster for the lifesteal amount
            logManager.AddLog(caster.name + " casted " + skill.skillName + " on " + target.name + " and healed for " + lifestealAmount + " HP.");
            target.takeTrueDamage(lifestealAmount); //deal true damage to the target for the lifesteal amount

        }
        else
        {
            logManager.AddLog(caster.name + " casted " + skill.skillName + " on " + target.name + " but failed to devour.");
        }



    }

    private IEnumerator doBestialAdrenaline(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        //add 1d6 + 2 to the attack of the user
        target.currentATK.AddDice(1, 6);
        target.currentATK.AddModifier(2);

        audioManager.PlaySound(skill.soundEffect); //play skill sound



        logManager.AddLog(caster.name + " casted " + skill.skillName);



        //////////////////////////////////////////////////

        StatusEffect inst = Instantiate(skill.statusEffect);
        target.activeSkillEffects.Add(inst); //add the skill to the active effects list

        activeEffectManager.AddEffect(inst,  caster, target, () => { }, () =>
        {

            target.activeSkillEffects.Remove(inst); //remove the effect from the active effects list

            target.currentATK.RemoveDice(1, 6, caster.currentATK); //remove 1d4 from the attack amount

            target.currentATK.AddModifier(-2); //remove the modifier

            DiceRoll damageAmount = new DiceRoll(); //create a new DiceRoll
            damageAmount.AddDice(1, 8); //
            int damage = damageAmount.Roll(); //roll the damage amount

            target.takeTrueDamage(damage); //deal true damage to the target

            logManager.AddLog(target.name + " takes " + damage + " damage from Beastial Adrenaline.");




        });

    }

    private IEnumerator doSpinalJaw(Entity target, Entity caster, Skill skill){

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play skill sound

        DiceRoll damageAmount = new DiceRoll(); //create a new DiceRoll
        damageAmount.AddDice(1, 8); //
        int damage = damageAmount.Roll(); //roll the damage amount

        int damageTaken = target.takeDamage(damage);

        if (damageTaken <= 0)
        {
            logManager.AddLog(caster.name + " casted " + skill.skillName + " on " + target.name + "but missed.");
        }
        else
        {
            logManager.AddLog(caster.name + " casted " + skill.skillName + " on " + target.name + " for " + damageTaken + " damage.");




           
            /// ////////////////////////////////////////////////////////////////////
            
            StatusEffect inst = Instantiate(skill.statusEffect);
            target.activeSkillEffects.Add(inst); //add the skill to the active effects list

            activeEffectManager.AddEffect(inst, caster, target, () =>
            {
                DiceRoll bleedDamage = new DiceRoll(); //create a new DiceRoll
                bleedDamage.AddDice(1, 4); //
                int Bdamage = bleedDamage.Roll(); //roll the damage amount

                target.takeTrueDamage(Bdamage); //deal true damage to the target
                
                logManager.AddLog(target.name + " took " + Bdamage + " bleed damage. ");
            },
            () =>
            {
                target.activeSkillEffects.Remove(inst); //remove the effect from the active effects list
            });


            

        }

       
    }



}
