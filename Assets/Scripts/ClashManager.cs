using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashManager : MonoBehaviour
{

    public static ClashManager Instance;
    private RoundManager roundManager;
    private AudioManager audioManager;
    private LogManager logManager;
    private QTE_Manager qteManager;


    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0,0,1,1);


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
        // Initialize any necessary components or variables here

        roundManager = RoundManager.Instance;
        audioManager = AudioManager.Instance;
        logManager = LogManager.Instance;
        qteManager = QTE_Manager.Instance;
    }


    public IEnumerator doBasicATK(Entity attacker, Entity target)
    {
        if (roundManager.player.isDead) { yield break; } // sops all atks after player death 

        yield return new WaitForSeconds(.7f);

        Vector3 startPos = attacker.transform.position;
        Vector3 endPos = target.transform.position;
        

        if (attacker.entityType != Entity.EntityType.Player) {
            
            yield return StartCoroutine(moveTo(attacker, target, startPos, endPos));

            attacker.GetComponent<Animator>().SetTrigger("Attacking"); 
        
        }


        // runs quick time event
        yield return StartCoroutine(qteManager.doQTE());
        bool qteSuceeded = qteManager.suceededQTE;
        

        if (attacker.entityType != Entity.EntityType.Player) yield return StartCoroutine(moveTo(attacker, target, endPos, startPos));


        
        


        //adds delay on atacks after the first one
        if (roundManager.clashQueue.actionQueue.Count > 1) { yield return new WaitForSeconds(3); }
        else { yield return new WaitForSeconds(.5F); }

        int damageDealt = 0;

        int attackRoll = attacker.currentATK.Roll(attacker.atkAdvantage);
        int targetRoll = target.currentATK.Roll(target.atkAdvantage);

        //if ( rolls + mod ) are equal, reroll
        while (attackRoll + attacker.currentATK.getModifier() == targetRoll + target.currentATK.getModifier())
        {
            audioManager.PlaySound(audioManager.atk_equal_sound);
            AnimationManager.Instance.doClashAnimation();
            yield return new WaitForSeconds(1f);
            Debug.Log("Rerolling ATK vs BLOCK");
            attackRoll = attacker.currentATK.Roll(attacker.atkAdvantage);
            targetRoll = target.currentATK.Roll(target.atkAdvantage);

        }

        //gets the crit and fail status of the rolls
        bool attackerCrit = attacker.currentATK.wasCriticalHit(attackRoll);
        bool targetCrit = target.currentATK.wasCriticalHit(targetRoll);
        bool attackerFail = attacker.currentATK.wasCriticalFail(attackRoll);
        bool targetFail = target.currentATK.wasCriticalFail(targetRoll);

        //gets the damage dealt based on the rolls and modifiers
        int atk = attackRoll + attacker.currentATK.getModifier();
        int block = targetRoll + target.currentATK.getModifier();

        bool doubleDamage = false;





        if ((attackerCrit && targetCrit) || (attackerFail && targetFail))
        {
            //does nothing
            Debug.Log("Both crit or fail, no damage dealt");

        }
        else if (attackerCrit && targetFail)
        {
            Debug.Log("Atacker crit and target crit failed");
            
            roundManager.clashQueue.Enqueue("ATKCRIT vs TARGETFAIL ", () => doBasicATK(attacker, target));

            //doubles the damage dealt
            doubleDamage = true;
        }
        else if (attackerCrit)
        {
            Debug.Log("Atacker crit ");
            
            roundManager.clashQueue.Enqueue("ATKCRIT ", () => doBasicATK(attacker, target));
        }
        // commented out for now, as for ballancing reasons: a light hitter can critblock too easily
        // else if (targetCrit)
        // {
        //     Debug.Log("Target crit");
        //     yield return new WaitForSeconds(1f);
        //     roundManager.clashQueue.Enqueue("TARGETCRIT ", () => doBasicATK(target, attacker));
        // }
        else if (attackerFail)
        {
            Debug.Log("Attacker fail");
            
            roundManager.clashQueue.Enqueue("TARGETCRIT ", () => doBasicATK(target, attacker));
        }
        else if (targetFail)
        {
            Debug.Log("target fail ");
            
            roundManager.clashQueue.Enqueue("ATKCRIT ", () => doBasicATK(attacker, target));
        }





        logManager.AddLog(attacker.name + ": " + atk + " VS " + target.name + ": " + block);
        //Debug.Log(attacker.name + ": " + attackRoll + " VS " + target.name + ": " + targetRoll);
        


        damageDealt = atk - block;
        if (doubleDamage) { damageDealt *= 2; }
        

        roundManager.animationManager.doSlashAnimation(target);

        //deals with QTE fail/sucess
        if (attacker.entityType == Entity.EntityType.Player)
        {
            if (qteSuceeded) { damageDealt += 3; }
            else { damageDealt /= 2; }
        }
        if (target.entityType == Entity.EntityType.Player)
        { 
            if (qteSuceeded) { damageDealt /= 2; }
            else { damageDealt += 3; }
        }



        float actualDamage = target.takeDamage(damageDealt);

        //camera shake when player takes damage
        if (actualDamage > 0 && target.entityType == Entity.EntityType.Player)
        { FindObjectOfType<CameraManager>().Shake(); }

        audioManager.PlayAttackSound(actualDamage);


    }





    public IEnumerator moveTo (Entity attacker, Entity target, Vector3 startPos, Vector3 endPos)
    {

        
        
         

        while (Vector3.Distance(attacker.transform.position, endPos) > 0.01f)
        {
           

            // uses an animation curve to move the attacker towards the target in a more dynamic way, rather than a linear movement
            // using bell curve for now
            float totalDistance = Vector3.Distance(startPos, endPos);
            float currentDistance = Vector3.Distance(attacker.transform.position, endPos);

            float curveT = moveCurve.Evaluate( currentDistance / totalDistance );


            attacker.transform.position = Vector3.Lerp(attacker.transform.position, endPos, curveT  );

            yield return null;
        }

        
    } 

}
