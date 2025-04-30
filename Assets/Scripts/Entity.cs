using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour
{
    private RoundManager roundManager;
    private SpriteRenderer spriteRenderer;
    private LogManager logManager; 
    private AudioManager audioManager;

    public enum EntityType { Player, Enemy, NPC };
    
    [SerializeField] public new string name;
    [SerializeField] private float hp;
    [SerializeField] private float maxHP;
    [SerializeField] private float mp;
    [SerializeField] private float maxMP;
    [SerializeField] public float def;
    [SerializeField] public DiceRoll baseATK = new DiceRoll();
    [SerializeField] public DiceRoll currentATK = new DiceRoll();
    [SerializeField] public int atkAdvantage = 0; //advantage for the attack roll
    [SerializeField] public int DEXTREZA = 1;
    [SerializeField] public int ATLETISMO = 1;

    [SerializeField] public EntityType entityType;
   
    [SerializeField] public List<Skill> skills = new List<Skill>();
    [HideInInspector]public List<Skill> skillsInstance = new List<Skill>();
    [SerializeField] public List<Skill> activeSkillEffects = new List<Skill>();

    


    public bool hasSupAction = true;
    


    public Entity() { }
    public Entity(float hp, float mp, float def, DiceRoll baseATK)
    {
        this.hp = hp;
        this.mp = mp;
        this.def = def;
        this.baseATK = baseATK;
        foreach (Dice dice in baseATK.dices) {
            this.currentATK.AddDice(dice.count, dice.sides);
        }
    }

    void Start()
    {

        roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        logManager = FindObjectOfType<LogManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();


        foreach (Dice dice in baseATK.dices) {
            this.currentATK.AddDice(dice.count, dice.sides);
        }

        foreach (Skill skill in skills) {
            skillsInstance.Add(Instantiate(skill)); //add the skill to the instance list
        }
       
    }

    public IEnumerator doBasicAtkCaller(Entity target) {

        yield return new WaitForSeconds(0f);

        roundManager.actionQueue.Enqueue("first atk", () => doBasicATK(target));
        
        

        while (roundManager.clashQueue.isRunning)
        {
            Debug.Log("null");
            yield return null;
        }

        roundManager.EndTurn();

        
        
    }
    private IEnumerator doBasicATK(Entity target){

        //adds delay on atacks after the first one
        if (roundManager.clashQueue.actionQueue.Count > 1) {yield return new WaitForSeconds(3);}
        else {yield return new WaitForSeconds(.5F);}

        var damage = doAtkClash(this, target);

        float actualDamage = target.takeDamage(damage);
        audioManager.PlayAttackSound();


    }
    public int doAtkClash(Entity attacker, Entity target) {
        int damageDealt = 0; 

        int attackRoll = attacker.currentATK.Roll(attacker.atkAdvantage);
        int targetRoll = target.currentATK.Roll(target.atkAdvantage);

        //if the rolls are equal, reroll)
        while(attackRoll == targetRoll) { 
            attackRoll = attacker.currentATK.Roll(attacker.atkAdvantage);
            targetRoll = target.currentATK.Roll(target.atkAdvantage);
        }

        ////////////////////FIX/////////////////////////////////////////////////////////////////////////////////////
        /// // //deals with crits and fails
        // if(attacker.currentATK.wasCriticalHit(attackRoll)) { 
        //     logManager.AddLog(attacker.name + " CRITHIT " + attacker.name + " rolled " + attackRoll + "|| "  + target.name + " rolled " + targetRoll); 
        //     roundManager.clashQueue.Enqueue("crithit", () => attacker.doBasicATK(target));
        //     }
        // else if (target.currentATK.wasCriticalHit(targetRoll)) {
        //     logManager.AddLog(target.name + " CRITHIT" + attacker.name + " rolled " + attackRoll + "|| "  + target.name + " rolled " + targetRoll); 
        //     roundManager.clashQueue.Enqueue("crithit", () =>  target.doBasicATK(attacker));
        //     }
        // else if (attacker.currentATK.wasCriticalFail(attackRoll)){
        //     logManager.AddLog(attacker.name + " CRITFAIL " +  attacker.name + " rolled " + attackRoll + "|| "  + target.name + " rolled " + targetRoll); 
        //     roundManager.clashQueue.Enqueue("critfail", () => target.doBasicATK(attacker));
        //     }
        // else if(target.currentATK.wasCriticalFail(targetRoll)){
        //     logManager.AddLog(target.name + " CRITFAIL " + attacker.name + " rolled " + attackRoll + "|| "  + target.name + " rolled " + targetRoll); 
        //     roundManager.clashQueue.Enqueue("critfail", () => attacker.doBasicATK(target));
        //     }
        // else {
        //     logManager.AddLog(attacker.name + " rolled " + attackRoll + "|| "  + target.name + " rolled " + targetRoll );
        // }
        logManager.AddLog(attacker.name + " rolled " + attackRoll + "|| "  + target.name + " rolled " + targetRoll );

        damageDealt = attackRoll - targetRoll; //calculate the damage dealt
        return damageDealt;


    }

    public int rollDEX() {
        int result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(DEXTREZA);
        return result;

    }   

    public float takeDamage(float damage) {

        if (roundManager == null) {roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();}
        if (spriteRenderer == null) {spriteRenderer = GetComponent<SpriteRenderer>();}

        float actualDamage = damage - def;
        if (actualDamage > 0) {
            hp -= damage - def;

            // Flash red to indicate damage taken
            roundManager.clashQueue.Enqueue("FlashRed",() => FlashSprite(spriteRenderer, Color.red));

            //show damage popup
            roundManager.clashQueue.Enqueue("showDamagePopup", () => roundManager.ShowDamagePopup(actualDamage, transform.position, Color.red));

        } else {
            actualDamage = 0;

            // Flash White to indicate block
            roundManager.clashQueue.Enqueue("Flashwhite",() => FlashSprite(spriteRenderer, Color.white ));

            //show damage popup
            roundManager.clashQueue.Enqueue("showDamagePopup", () => roundManager.ShowDamagePopup(actualDamage, transform.position, Color.gray));
        }


        if (hp <= 0) {die();}

        return actualDamage; //return the actual damage taken 
         
        


        
    }

    public void takeTrueDamage(float damage) {
        hp -=  damage;
        if (hp <= 0) {die();}
    }

    public void heal(float value) {
        hp += value;
    }

    public void loseMP(float value) {
        mp -= value;
    }

    public void gainMP(float value) {
        mp += value;
    }

    public void SetDef(float value) {
        def += value;
    }

    public void AddSkill(Skill skill) {
        skillsInstance.Add(skill);
    }

    public void RemoveSkill(Skill skill) {
        skillsInstance.Remove(skill);
    }

    public void die() {
        spriteRenderer.color = Color.black; //change the color of the sprite to gray
        Debug.Log(name + " has died.");
    }

    public bool hasEffect(Skill skill) {
        foreach (Skill effect in activeSkillEffects) {
            if (effect == skill) {
                return true; // Return true if the effect is found
            }
        }
        return false; // Return false if the effect is not found
    }


    //flash the entity a inputed color
    public IEnumerator FlashSprite(SpriteRenderer spriteRenderer, Color color, float duration = 0.2f)
    {
        // Get the original material and set the new material
        Material originalMaterial = GetComponent<Renderer>().material;
        originalMaterial = GetComponent<Renderer>().material = originalMaterial;

        spriteRenderer.material = Resources.Load<Material>("Materials/whiteMaterial");

        // Set the color to the new color
        Color original = spriteRenderer.color;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(duration);

        // Reset the material and color to original
        spriteRenderer.material = originalMaterial;
        spriteRenderer.color = original;

    }

    public static explicit operator List<object>(Entity v)
    {
        throw new NotImplementedException();
    }

    
    public float getHP() {
        return hp;
    }
    public float getMP() {
        return mp;
    }
    public float getMaxHP() {
        return maxHP;
    }
    public float getMaxMP() {
        return maxMP;
    }
    
    public string getStatusAsString(){
        string stts = "";

        stts += "Name: " + name + "\n";
        stts +="HP: "+ hp + "/" +maxHP + "\n";
        stts +="MP " +mp + "/" +maxMP + "\n";
        stts +="DEF: "+ def +"\n";
        stts += "Base ATK: " +baseATK.diceToString() + "\n";
        stts += "Current ATK: " + currentATK.diceToString() + "\n";
        stts += "Current ATK Advantages: " + atkAdvantage + "\n";
        stts += "DEXTERITY: " + DEXTREZA + "\n";
        stts += "ATHLEtics: " + ATLETISMO + "\n";



        return stts;

    }

}
