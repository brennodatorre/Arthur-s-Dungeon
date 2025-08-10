using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

[System.Serializable]
public class Entity : MonoBehaviour
{
    private RoundManager roundManager;
    private SpriteRenderer spriteRenderer;
    private LogManager logManager; 
    private AudioManager audioManager;

    public enum EntityType { Player, Enemy, NPC };
    
    [Space]
    [Header("Base Status")]
    [SerializeField] public EntityType entityType;
    [SerializeField] public new string name;
    [SerializeField] private float hp;
    [SerializeField] private float maxHP;
    [SerializeField] private float mp;
    [SerializeField] private float maxMP;
    [SerializeField] public float def;



    public enum Trait { DEXTREZA, ATLETISMO, AURA, CARISMA, SORTE, INTUICAO, HEX, ASTUCIA, VONTADE, REFLEXOS, PERSEPCAO, FURTIVIDADE, CONSTITUICAO };
    [Space]
    [Header("Traits")]
    

    [SerializeField] public int DEXTREZA = 1;
    [SerializeField] public int ATLETISMO = 1;
    [SerializeField] public int AURA = 1;
    [SerializeField] public int CHARISMA = 1;
    [SerializeField] public int SORTE = 1;
    [SerializeField] public int INTUICAO = 1;
    [SerializeField] public int HEX = 1;
    [SerializeField] public int ASTUCIA = 1;
    [SerializeField] public int VONTADE = 1;
    [SerializeField] public int REFLEXOS = 1;
    [SerializeField] public int PERSEPCAO = 1;
    [SerializeField] public int FURTIVIDADE = 1;
    [SerializeField] public int CONSTITUICAO = 1;
    [SerializeField] public int DOMINANCIA = 1;


    [Space]
    [Header("Atk Status")]

    [SerializeField] public DiceRoll baseATK = new DiceRoll();
    [SerializeField] public DiceRoll currentATK = new DiceRoll();
    [SerializeField] public int atkAdvantage = 0; //advantage for the attack roll


    [Space]
    [Header("States")]
    
    public bool isDead = false;
    public bool hasSupAction = true;
    


    [Space]
    [Header("Skiils")]

    [SerializeField] public List<Skill> skills = new List<Skill>();
    [HideInInspector]public List<Skill> skillsInstance = new List<Skill>(); // to not edit original copy
    [SerializeField] public List<Skill> activeSkillEffects = new List<Skill>();

    


    
    


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

        roundManager = GameObject.Find("CombatManager").GetComponent<RoundManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        logManager = FindObjectOfType<LogManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        if(baseATK.dices.Count ==0) {baseATK.AddDice(0,0);}

        //copies baseATK dices to currentATK
        foreach (Dice dice in baseATK.dices) {
            this.currentATK.AddDice(dice.count, dice.sides);
        }

        
        currentATK.AddModifier (baseATK.getModifier());

        //clear the skill instance so there wont be duplicates
        skillsInstance.Clear(); 

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

        roundManager.animationManager.doAnimation(target);

        float actualDamage = target.takeDamage(damage);

        //camera shake when player takes damage
        if (actualDamage > 0 && target.entityType == EntityType.Player) 
        {FindObjectOfType<CameraManager>().Shake();}

        audioManager.PlayAttackSound(actualDamage);




    }
    public int doAtkClash(Entity attacker, Entity target) {
        int damageDealt = 0; 


        int attackRoll = attacker.currentATK.Roll(attacker.atkAdvantage);
        int targetRoll = target.currentATK.Roll(target.atkAdvantage);



        //if the rolls are equal, reroll)
        while(attackRoll == targetRoll ) { 
            attackRoll = attacker.currentATK.Roll(attacker.atkAdvantage);
            targetRoll = target.currentATK.Roll(target.atkAdvantage);

        }


        if (attackRoll < 0) {attackRoll = 0;}
        if (targetRoll < 0) { targetRoll = 0;}

        

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

        logManager.AddLog(attacker.name + " rolled " + attackRoll + " || "  + target.name + " rolled " + targetRoll );


        damageDealt = attackRoll + attacker.currentATK.getModifier() - (targetRoll + target.currentATK.getModifier()); //calculate the damage dealt
        return damageDealt;


    }

    public int rollDEX() {
        int result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(DEXTREZA);
        return result;

    }

    //rolls a test based on inputed trait
    public int rollTest(Trait trait)
    {
        int result = 0;

        switch (trait)
        {
            case Trait.DEXTREZA:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(DEXTREZA - 1);
                break;
            case Trait.ATLETISMO:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(ATLETISMO - 1);
                break;
            case Trait.AURA:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(AURA - 1);
                break;
            case Trait.CARISMA:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(CHARISMA - 1);
                break;
            case Trait.SORTE:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(SORTE - 1);
                break;
            case Trait.INTUICAO:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(INTUICAO - 1);
                break;
            case Trait.HEX:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(HEX - 1);
                break;
            case Trait.ASTUCIA:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(ASTUCIA - 1);
                break;
            case Trait.VONTADE:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(VONTADE - 1);
                break;
            case Trait.REFLEXOS:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(REFLEXOS - 1);
                break;
            case Trait.PERSEPCAO:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(PERSEPCAO - 1);
                break;
            case Trait.FURTIVIDADE:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(FURTIVIDADE - 1);
                break;
            case Trait.CONSTITUICAO:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(CONSTITUICAO - 1);
                break;
        }

        return result;

    } 

    public float takeDamage(float damage)
    {

        if (roundManager == null) { roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>(); }
        if (spriteRenderer == null) { spriteRenderer = GetComponent<SpriteRenderer>(); }

        float actualDamage = damage - def;
        if (actualDamage > 0)
        {
            hp -= damage - def;

            if (hp <= 0) { die(); }
            else
            {

                // Flash red to indicate damage taken
                roundManager.clashQueue.Enqueue("FlashRed", () => FlashSprite(spriteRenderer, Color.red));

                //show damage popup
                roundManager.clashQueue.Enqueue("showDamagePopup", () => roundManager.ShowDamagePopup(actualDamage, transform.position, Color.red));
            }
        }
        else
        {
            actualDamage = 0;

            // Flash White to indicate block
            roundManager.clashQueue.Enqueue("Flashwhite", () => FlashSprite(spriteRenderer, Color.white));

            //show damage popup
            roundManager.clashQueue.Enqueue("showDamagePopup", () => roundManager.ShowDamagePopup(actualDamage, transform.position, Color.gray));
        }




        Debug.Log(damage + " || " + actualDamage);

        return actualDamage; //return the actual damage taken 





    }

    public void takeTrueDamage(float damage) {
        hp -=  damage;
        if (hp <= 0) {die();}
    }

    public void heal(float value) {
        hp += value;
        if (hp > maxHP) {hp = maxHP;}
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

        isDead = true;
        //spriteRenderer.color = Color.black; //change the color of the sprite to gray
        Debug.Log(name + " has died.");

        // If player Dies
        if (this.entityType == EntityType.Player)
        {

            roundManager.actionQueue.actionQueue.Clear(); // clears next actions on action queue

            PlayerData.Instance.fablePoints++;
            PlayerData.Instance.death_counter++;

            //goes to fable shop on player death
            StartCoroutine(GameObject.FindObjectOfType<MySceneManager>().openSceneWithTransition("DEATHSHOP", true));

        }
        else
        { // if not player



            //removes the dead entity from theirs respective arrays
            roundManager.entities = roundManager.entities.Where(x => x != this).ToList();

            if (this.entityType == EntityType.Enemy)
            {

                roundManager.enemies = roundManager.enemies.Where(x => x != this).ToArray();

                if (roundManager.enemies.Length == 0)
                {  //goes to tutorial if there are no more enemies
                    StartCoroutine(GameObject.FindObjectOfType<MySceneManager>().openSceneWithTransition("COMBAT", true));
                }


                PlayerData.Instance.fablePoints++;
                PlayerData.Instance.kill_counter++;
            }
            else
            { //is an ally
                roundManager.allies = roundManager.allies.Where(x => x != this).ToArray();
            }


            StartCoroutine(DissolveUponDeath()); //dissolves the entity upon death

        
        
        }
        

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

    //does Dissolve effect on the entity upon death
    public IEnumerator DissolveUponDeath()
    {

        Material mat = spriteRenderer.material;

        float fade = 1;

        while (fade > 0)
        {
            fade -= Time.deltaTime * 0.5f; //fade out over time
            mat.SetFloat("_Fade", fade);
            yield return null; //wait for next frame
        }

    }







    ////////////////////////////////////// Getters and Setters for HP, MP, Max HP, and Max MP ////////////////////////////////////////////////////


    public float getHP()
    {
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
    public void setHP(float x) {
        hp = x;
    }
    public void setMP(float x) {
        mp = x;
    }
    public void setMaxHP(float x) {
        maxHP = x;
    }
    public void setMaxMP(float x) {
        maxMP = x;
        
    }
    

    public string getStatusAsString(){
        string stts = "";

        stts += "Name: " + name + "\n \n";
        stts +="HP: "+ hp + "/" +maxHP + "\n";
        stts +="MP " +mp + "/" +maxMP + "\n\n";
        stts +="DEF: "+ def +"\n";
        stts += "Base ATK: " +baseATK.diceToString() + "\n";
        stts += "Current ATK: " + currentATK.diceToString() + "\n";
        stts += "Current ATK Advantages: " + atkAdvantage + "\n \n";
        stts += "DEXTERITY: " + DEXTREZA + "\n";
        stts += "ATHLEtics: " + ATLETISMO + "\n";

        stts += "Has Supporting Action = " +hasSupAction + "\n";



        return stts;

    }

}
