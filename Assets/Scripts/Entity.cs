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
    
    [SerializeField] private float hp;
    [SerializeField] private float maxHP;
    [SerializeField] private float mp;
    [SerializeField] private float maxMP;
    [SerializeField] public float def;
    [SerializeField] public DiceRoll baseATK = new DiceRoll();
    [SerializeField] public DiceRoll currentATK = new DiceRoll();
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

    public IEnumerator doBasicATK(Entity target) {

        yield return new WaitForSeconds(0f);

        int damage = currentATK.Roll();
        float actualDamage = target.takeDamage(damage);
        audioManager.PlayAttackSound();

        logManager.AddLog( name + " atacked " + target.name + " for " + actualDamage + " damage.");

        roundManager.EndTurn();
        
    }

    public int rollDEX() {
        int result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).RollWithAdvantage(DEXTREZA);
        return result;

    }   

    public float takeDamage(float damage) {

        if (roundManager == null) {roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();}
        if (spriteRenderer == null) {spriteRenderer = GetComponent<SpriteRenderer>();}

        float actualDamage = damage - def;
        if (actualDamage > 0) {
            hp -= damage - def;

            // Flash red to indicate damage taken
            roundManager.actionQueue.Enqueue("FlashRed",() => FlashSprite(spriteRenderer, Color.red));

            //show damage popup
            roundManager.actionQueue.Enqueue("showDamagePopup", () => roundManager.ShowDamagePopup(actualDamage, transform.position, Color.red));

        } else {
            actualDamage = 0;

            // Flash White to indicate block
            roundManager.actionQueue.Enqueue("Flashwhite",() => FlashSprite(spriteRenderer, Color.white ));

            //show damage popup
            roundManager.actionQueue.Enqueue("showDamagePopup", () => roundManager.ShowDamagePopup(actualDamage, transform.position, Color.gray));
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
    

}
