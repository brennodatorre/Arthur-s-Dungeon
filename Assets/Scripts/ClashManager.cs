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


    // public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0,0,1,1);
 
    [Header("Attack Movement")]
    [SerializeField] private float duration = 0.25f;

    [Range(0.1f, 5f)]
    [SerializeField] private float easeInPower = 2f;

    [Range(0.1f, 5f)]
    [SerializeField] private float easeOutPower = 1.5f;

[Range(0f, 1f)]
[SerializeField] private float punchStrength = 0.6f;

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
        

        // moves attacker to target position and plays attack animation
        if (attacker.entityType != Entity.EntityType.Player) {
            
            yield return StartCoroutine(moveTo(attacker, target, startPos, endPos));

            attacker.GetComponent<Animator>().SetTrigger("Attacking"); 
        
        }


        // runs quick time event
        yield return StartCoroutine(qteManager.doQTE());
        bool qteSuceeded = qteManager.suceededQTE;
        
        // enemy moves back to original position after attack
        if (attacker.entityType != Entity.EntityType.Player) yield return StartCoroutine(moveTo(attacker, target, endPos, startPos));



        //adds delay on atacks after the first one
        if (roundManager.clashQueue.actionQueue.Count > 1) { yield return new WaitForSeconds(3); }
        else { yield return new WaitForSeconds(.5F); }




       

        (int attackRoll, bool attackerCrit, bool attackerFail) = attacker.currentATK.RollWithCritCheck(attacker.atkAdvantage);
        (int targetRoll, bool targetCrit, bool targetFail) = target.currentATK.RollWithCritCheck(target.atkAdvantage);


        int rerolls = 0;
        //if rolls are equal, reroll, max 100
        while (attackRoll  == targetRoll && rerolls<100)
        {
            rerolls++;

            audioManager.PlaySound(audioManager.atk_equal_sound);
            AnimationManager.Instance.doClashAnimation();
            yield return new WaitForSeconds(1f);
            Debug.Log("Rerolling ATK vs BLOCK");

            (attackRoll, attackerCrit,  attackerFail) = attacker.currentATK.RollWithCritCheck(attacker.atkAdvantage);
            (targetRoll, targetCrit,  targetFail) = target.currentATK.RollWithCritCheck(target.atkAdvantage);

        }


        #region Crit Handling

        bool CritFailDoubleDamage = false;

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
            CritFailDoubleDamage = true;
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


        #endregion




        
        string atkAdds = "";
        int modedAttackRoll = attackRoll;
        
        if (CritFailDoubleDamage) { modedAttackRoll *= 2;  atkAdds += " *2 Crit/Fail"; }
        

        

        //deals with QTE fail/sucess
        if (attacker.entityType == Entity.EntityType.Player)
        {
            if (qteSuceeded) { modedAttackRoll += 3;  atkAdds += " + 3 QTE"; }
            else { modedAttackRoll = Mathf.CeilToInt(modedAttackRoll / 2f); atkAdds += " /2 QTE"; }
        }
        if (target.entityType == Entity.EntityType.Player)
        { 
            if (qteSuceeded) { modedAttackRoll = Mathf.CeilToInt(modedAttackRoll / 2f); atkAdds += " /2 QTE"; }
            else { modedAttackRoll += 3;  atkAdds += " + 3 QTE"; }
        }


        
        float damageDealt = target.takeDamage(modedAttackRoll - targetRoll);


        roundManager.animationManager.doSlashAnimation(target);
        logManager.AddLog(attacker.name + ": " + attackRoll + atkAdds + " VS " + target.name + ": " + targetRoll+ " | Damage Dealt: " + damageDealt );


        //camera shake when player takes damage
        if (damageDealt > 0 && target.entityType == Entity.EntityType.Player)
        { FindObjectOfType<CameraManager>().Shake(); }

        audioManager.PlayAttackSound(damageDealt);


    }





    public IEnumerator moveTo (Entity attacker, Entity target, Vector3 startPos, Vector3 endPos)
    {  

        while (Vector3.Distance(attacker.transform.position, endPos) > 0.01f)
        {
                
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                float easedT = AttackEase(t);

                attacker.transform.position = Vector3.Lerp(startPos, endPos, easedT);

                yield return null;
            }

            attacker.transform.position = endPos;
        }

        
    } 

    private float AttackEase(float t)
    {
        // ease in (wind-up)
        float easeIn = Mathf.Pow(t, easeInPower);

        // ease out (impact control)
        float easeOut = 1f - Mathf.Pow(1f - t, easeOutPower);

        // base blend
        float baseCurve = Mathf.Lerp(easeIn, easeOut, 0.5f);

        // punch / lunge feel
        float punch = Mathf.Sin(t * Mathf.PI) * punchStrength;

        float result = baseCurve + punch * (1f - t);

        return Mathf.Clamp01(result);
    }

}
