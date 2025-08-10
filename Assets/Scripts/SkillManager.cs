using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SkillManager : MonoBehaviour
{

    public RoundManager roundManager;
    public LogManager logManager;
    public AudioManager audioManager;
    public ActiveEffectManager activeEffectManager;

    public List<Skill> skills = new List<Skill>();


    public void doSkill(Entity target, Entity caster, Skill skill)
    {

        if (skill.CanBeUsed(caster, target))
        {
            //use up the sup action if the skill is a support action
            if (skill.isSupportAction) { caster.hasSupAction = false; }



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
            else
            {
                Debug.Log("Skill not implemented yet.");
            }

        }
        else { audioManager.PlaySound(audioManager.skill_unable_sound); }

        roundManager.buttonManager.closeSkillMenu(); //close the skill menu after the action is done

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
        caster.loseMP(skill.mpCost); //lose 5 MP for casting the skill

        logManager.AddLog(caster.name + " casted Healing Tear on " + target.name + " for " + heal + " HP.");

    }

    private IEnumerator doRottingTouch(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        caster.loseMP(skill.mpCost); //lose 5 MP for casting the skill

        audioManager.PlaySound(skill.soundEffect); //play skill sound

        DiceRoll damageAmount = new DiceRoll(); //create a new DiceRoll
        damageAmount.AddDice(1, 4); //

        target.activeSkillEffects.Add(skill); //add the skill to the active effects list

        target.currentATK.AddDice(1, 4); //add 1d4 to the attack amount

        logManager.AddLog(caster.name + " casted " + skill.skillName + " on " + target.name);



        //////////////////////////////////////////////////

        activeEffectManager.AddEffect(skill, 1, RoundManager.TurnPhase.End, () => { }, () =>
        {

            target.activeSkillEffects.Remove(skill); //remove the effect from the active effects list

            target.currentATK.RemoveDice(1, 4, caster.currentATK); //remove 1d4 from the attack amount


        });

    }

    private IEnumerator doPlatedSoul(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        caster.loseMP(skill.mpCost); //lose MP for casting the skill

        audioManager.PlaySound(skill.soundEffect); //play skill sound



        target.activeSkillEffects.Add(skill); //add the skill to the active effects list

        target.def += 3; //add 3 to the defense amount

        logManager.AddLog(caster.name + " casted " + skill.skillName + " on " + target.name);



        //////////////////////////////////////////////////

        activeEffectManager.AddEffect(skill, 3, RoundManager.TurnPhase.Start, () => { }, () =>
        {
            target.activeSkillEffects.Remove(skill); //remove the effect from the active effects list

            target.def += -3; //remove 1d4 from the attack amount
        }


        );

    }

     private IEnumerator doDevour(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play the skill sound

        int casterRoll = 0;
        if (caster.DEXTREZA > caster.ATLETISMO) {casterRoll = caster.rollTest(Entity.Trait.DEXTREZA);} //roll a test based on the caster's DEXTREZA
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

}
