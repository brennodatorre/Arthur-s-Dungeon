using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Properties;
using static StatusEffect;

public class SkillManager : MonoBehaviour
{
    [HideInInspector] public static SkillManager Instance;

    public RoundManager roundManager;
    public LogManager logManager;
    public AudioManager audioManager;
    public ActiveEffectManager activeEffectManager;

    
    private SkillDatabase _allSkillDatabase;

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

    void Start()
    {
        _allSkillDatabase = DatabaseManager.Instance.allSkillsDatabase;
    }




    public void doSkill(Entity target, Entity caster, Skill skill)
    {

        bool isTargettingEnemy = RoundManager.Instance.enemies.Contains(target);

        // deals with self targeting skills (casting a self target into someone else)
        if (target != caster && skill.targetType == Target.Self)
        {
            audioManager.PlaySound(audioManager.skill_unable_sound);
            logManager.AddLog(skill.skillName + " can only be used on yourself.");
            roundManager.buttonManager.closeSkillMenu(); //close the skill menu after the action is done
            return;
        }
        

        

        if (skill.CanBeUsed(caster, target))
        {
            //use up the sup action if the skill is a support action
            if (skill.actionType == ActionType.Sup) { caster.currentSupActions--; }
            else if (skill.actionType == ActionType.Main) { caster.currentMainActions--; }

            caster.changeMP( - skill.mpCost); //caster lose mp

            //deals with the skill's fulldescription if it has one
            if (caster.entityType == Entity.EntityType.Player && skill.hasBeenUsed == false)
            {
                skill.hasBeenUsed = true;
            }

            switch (skill.skillID) { 

                case "s_healingTear":
                {

                    roundManager.actionQueue.Enqueue("do HealingTear", () => doHealingTear(target, caster, skill)); //add the action to the queue
                    break;

                }
                case "s_rottingTouch":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doRottingTouch(target, caster, skill)); //add the action to the queue
                    break;
                }
                case "s_platedSoul":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doPlatedSoul(target, caster, skill)); //add the action to the queue
                    break;
                }
                case "s_devour":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doDevour(target, caster, skill)); //add the action to the queue
                    break;
                }
                case "s_bestialAdrenaline":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doBestialAdrenaline(target, caster, skill)); //add the action to the queue
                    break;
                }
                case "s_spinalJaw":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doSpinalJaw(target, caster, skill)); //add the action to the queue
                    break;
                }
                case "s_suicide":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doSuicide(target, caster, skill)); //add the action to the queue
                    break;
                }
                case "s_electrifyWeapon":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doElectrifyWeapon(target, caster, skill)); //add the action to the queue
                    break;
                }
                case "s_rest":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doRest( caster, skill)); //add the action to the queue
                    break;
                }
                case "s_prepare":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doPrepare( caster, skill)); //add the action to the queue
                    break;
                }
                case "s_GamblerGambit":
                {

                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doGamblerGambit( caster, skill)); //add the action to the queue
                    break;
                }
                case "s_bodyAsAShield":
                {
                    roundManager.actionQueue.Enqueue("do " + skill.skillName, () => doBodyAsAShield(target, caster, skill)); //add the action to the queue
                    break;
                }
                default:
                {
                    Debug.Log("Skill not implemented yet.");
                    break;

                }

            }

        }
       

        if (caster.currentMainActions <= 0)
        {
            if (caster.entityType == Entity.EntityType.Player) {
                roundManager.buttonManager.closeSkillMenu(); //close the skill menu after the action is done
                roundManager.act_menu.SetActive(false); //close the action menu after the action is done
            }
            
            roundManager.EndTurn(); //end the turn if the caster has no main actions left
        }
        else
        {
            //roundManager.EnableSkillTargetingUI(true);
            if (caster.entityType == Entity.EntityType.Player) roundManager.buttonManager.skillMenu.SetActive(true);
            else { roundManager.EndTurn();}
        }



    }


    #region Basic Skill Management

    public Skill getSkill(string skillName)
    {
        foreach (Skill skill in _allSkillDatabase.skills)
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
            logManager.AddLog(caster.entityName + " deoes not have enough MP to cast Healing Tear.");
            return false; //return false if the caster does not have enough MP to cast the skill
        }

        return true; //return true if the caster has enough MP to cast the skill
    }

    public void resetSkills(Entity caster)
    {
        foreach (Skill skill in caster.skills)
        {
            skill.ResetUsesPerTurn(); //reset the uses per turn for each skill
        }
    }


    #endregion










    #region Skill Effects



    private IEnumerator doHealingTear(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play the healing sound
        target.GetComponent<VisualEffectManager>().PlayParticleEffect(target.GetComponent<VisualEffectManager>().healingParticleSystem); //play the healing particle effect on the target

        var heal = skill.mainDice.Roll(); 
        target.heal(heal); //heal the target 


        logManager.AddLog(caster.entityName + " casted Healing Tear on " + target.entityName + " for " + heal + " HP.");

    }

    private IEnumerator doRottingTouch(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds


        activeEffectManager.addRottingTouch(target, caster, skill, true); //add rotting touch to the target
        logManager.AddLog(caster.entityName + " casted " + skill.skillName + " on " + target.entityName);


    }

    private IEnumerator doElectrifyWeapon(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds


        activeEffectManager.addElectrifiedWeapon(target, caster, skill,true); //add electrified weapon to the target
        logManager.AddLog(caster.entityName + " casted " + skill.skillName + " on " + target.entityName);
        

    }

    private IEnumerator doPrepare( Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        ActiveEffectManager.Instance.addPrepared(caster, caster, skill, true); //add prepared to the target

    }


    private IEnumerator doPlatedSoul(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        

        activeEffectManager.addPlattedSoul(target, caster, skill,true); //add platted soul to the target
        logManager.AddLog(caster.entityName + " casted " + skill.skillName + " on " + target.entityName);

    }

    private IEnumerator doDevour(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play the skill sound

        int casterRoll = 0;
        if (caster.DEX > caster.ATLETISM) { casterRoll = caster.rollTest(caster.DEX); } //roll a test based on the caster's DEXTREZA
        else { casterRoll = caster.rollTest(caster.ATLETISM); } //roll a test based on the caster's ATLETISMO

        int targetRoll = target.rollTest(target.REFLEX); //roll a test based on the target's CONSTITUICAO

        //if skill sucesseds
        if (casterRoll > targetRoll)
        {
            DiceRoll lifesteal = new DiceRoll(); //create a new DiceRoll for the lifesteal amount
            lifesteal.AddDice(1, 4); //add 1d4 to
            int lifestealAmount = lifesteal.Roll(); //roll the lifesteal amount
            caster.heal(lifestealAmount); //heal the caster for the lifesteal amount
            logManager.AddLog(caster.entityName + " casted " + skill.skillName + " on " + target.entityName + " and healed for " + lifestealAmount + " HP.");
            target.takeTrueDamage(lifestealAmount); //deal true damage to the target for the lifesteal amount

        }
        else
        {
            logManager.AddLog(caster.entityName + " casted " + skill.skillName + " on " + target.entityName + " but failed to devour.");
        }



    }

    private IEnumerator doBestialAdrenaline(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        ActiveEffectManager.Instance.addBestified(target, caster, skill,true); //add bestified to the target
        logManager.AddLog(caster.entityName + " casted " + skill.skillName);

    }

    private IEnumerator doSpinalJaw(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play skill sound

        DiceRoll damageAmount = new DiceRoll(); //create a new DiceRoll
        damageAmount.AddDice(1, 8); //
        int damage = damageAmount.Roll(); //roll the damage amount

        int damageTaken = target.takeDamage(damage);

        if (damageTaken <= 0)
        {
            logManager.AddLog(caster.entityName + " casted " + skill.skillName + " on " + target.entityName + "but missed.");
        }
        else
        {
            logManager.AddLog(caster.entityName + " casted " + skill.skillName + " on " + target.entityName + " for " + damageTaken + " damage.");



            if (!target.isDead) activeEffectManager.addBleed(target, caster, skill,true);



        }


    }

    private IEnumerator doSuicide(Entity target, Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play skill sound


        target.takeTrueDamage (999999999); //add 3 to the defense amount

        logManager.AddLog(caster.entityName + " casted " + skill.skillName + " on " + target.entityName);


    }

    private IEnumerator doRest( Entity caster, Skill skill)
    {

        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play the healing sound

        
        

        var recover = skill.mainDice.Roll(); //roll the healing amount
        caster.changeMP(recover); //heal the target for 10 HP


        logManager.AddLog(caster.entityName + " rested and recovered" + recover + " MP.");

    }

    private IEnumerator doGamblerGambit( Entity caster, Skill skill)
    {
        
        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play the skill sound

        int damage =  caster.getHP() - 1 ;

        foreach (var ent in RoundManager.Instance.entities)
        {
            ent.takeDamage(damage);
        }

        logManager.AddLog(caster.entityName + " used Gambler's Gambit");


    }


    private IEnumerator doBodyAsAShield(Entity target, Entity caster, Skill skill)
    {
        
        yield return new WaitForSeconds(0f); //wait for 0 seconds

        audioManager.PlaySound(skill.soundEffect); //play the skill sound

        ActiveEffectManager.Instance.addBodyShielded(target, caster, skill,true); //add body as a shield to the target

        logManager.AddLog(caster.entityName + " used Body as a Shield on " + target.entityName); ;

    }


    #endregion


}
