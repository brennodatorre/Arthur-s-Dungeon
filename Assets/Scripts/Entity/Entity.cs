
using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;
using Random = UnityEngine.Random;  

using Image = UnityEngine.UI.Image;
using static StatusEffect;

using static Properties;

[System.Serializable]
public class Entity : MonoBehaviour
{
    public bool IsAClass = false;
    private RoundManager roundManager;
    private Image sprite;
    private LogManager logManager; 
    private AudioManager audioManager;
    [HideInInspector] public GameObject spawnPoint;

    public Brain brain;
    

    public enum EntityType { Player, Enemy, NPC };
    


    public GameObject statEffectDisplay;


    [Space(5)]
    [Header("Base Status")]
    [SerializeField] public EntityType entityType;
    public List<Property> properties = new List<Property>();
    public Origin entityOrigin;
    [SerializeField] public string entityName;
    public string entityID;
    [SerializeField] private int hp;
    [SerializeField] private int maxHP;
    [SerializeField] private int mp;
    [SerializeField] private int maxMP;
    [SerializeField] private int def;




    
    [Space(10)]
    [Header("Traits (0 = 1d20)")]
    

    [SerializeField] public int DEX = 0;
    [SerializeField] public int ATLETISM = 0;
    [SerializeField] public int AURA = 0;
    [SerializeField] public int CHARISM =0;
    [SerializeField] public int LUCK = 0;
    [SerializeField] public int INTUITION = 0;
    [SerializeField] public int HEX = 0;
    [SerializeField] public int INT = 0;
    [SerializeField] public int WILL = 0;
    [SerializeField] public int REFLEX = 0;
    [SerializeField] public int PERSEPTION = 0;
    [SerializeField] public int FURTIVITY = 0;
    [SerializeField] public int CONSTITUTION = 0;
    [SerializeField] public int DOMINANCE = 0;


    [Space(10)]
    [Header("Atk Status")]

    [SerializeField] public DiceRoll baseATK = new DiceRoll();
    [SerializeField] public DiceRoll currentATK = new DiceRoll();

    [SerializeField] public int atkAdvantage = 0; //advantage for the attack roll


    [Space(10)]
    [Header("States")]
    
    public bool isDead = false;
    public int totalMainActions = 1;
    public int totalSupActions = 1;

    public int currentMainActions = 1;
    public int currentSupActions = 1;
    


    [Space(10)]
    [Header("Skiils")]

    [SerializeField]public List<Skill> skills = new List<Skill>(); // to not edit original copy
    [SerializeField] public List<StatusEffect> activeStatusEffects = new List<StatusEffect>();


    [Space(10)]
    [Header("Items")]
    [SerializeField] public List<Item> items = new List<Item>(); // to not edit original copy

    [Space(10)]
    [Header("Fable Status")]
    public int fableWorth;

    
    


    [HideInInspector] public Image crackingSpriteOverlay;

    public Entity() { }
    public Entity(int hp, int mp, int def, DiceRoll baseATK)
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
        if (IsAClass) return; // if this entity is just a class template, dont run the start method since it doesnt have a sprite or needs to be registered in the round manager

        roundManager = GameObject.Find("CombatManager").GetComponent<RoundManager>();
        sprite = GetComponent<Image>();
        logManager = FindObjectOfType<LogManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();




        sprite.material = MaterialPallet.Instance.getColoredMaterial(
            MaterialPallet.Instance.getOriginColor(entityOrigin), 
            MaterialPallet.Instance.dissolveMaterial
        );




        if (baseATK.dices.Count == 0) { baseATK.AddDice(0, 0); }

        //copies baseATK dices to currentATK
        foreach (Dice dice in baseATK.dices)
        {
            this.currentATK.AddDice(dice.count, dice.sides);
        }
        currentATK.AddModifier(baseATK.getModifier());

        
        
        if (entityType != EntityType.Player) {brain = GetComponent<Brain>(); }

        
       
    }

    public IEnumerator doBasicAtkCaller(Entity target, bool ignoreOveride = false)
    {
        roundManager.currentPhase = RoundManager.TurnPhase.Clash;

        yield return new WaitForSeconds(0f);


        

        //OVERRIDE CHECK
        StatusEffect BlockerOverideEffect = null;
        if (!ignoreOveride)
        {
            BlockerOverideEffect = target.CheckForOverideInStatusEffects(OverideEffectType.BLOCK);
        }

        if (BlockerOverideEffect != null )
        {
            BlockerOverideEffect?.overideEffectAct?.Invoke(new object[] { this ,target }); // Invoke the override action if it exists
           yield break; // Exit the method if an override effect is found
        }


        roundManager.clashQueue.Enqueue("first atk", () => ClashManager.Instance.doBasicATK(this, target));


        //while combat is running, dont end the action/turn 
        while (roundManager.clashQueue.isRunning)
        {
            //Debug.Log("null");
            yield return null;
        }


        roundManager.currentTurn.currentMainActions--; //decrease the main actions left for the entity
        
        if (roundManager.playerIsAttacking) { roundManager.playerIsAttacking = false; } //set the player as not attacking anymore

        if (roundManager.currentTurn.currentMainActions < 1)
        {
            roundManager.EndTurn();
        }
        else
        {

            if (roundManager.currentTurn == roundManager.player) //if its the players turn, brind up the action menu
            {
                roundManager.act_menu.SetActive(true);

            }
            roundManager.currentPhase = RoundManager.TurnPhase.Action; //set the current phase to action
        }

        
        
    }



    public int rollDEX() {
        int result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(DEX);
        return result;

    }

    /// <summary>
    /// Rolls a Trait test based on the Trait Scaling
    /// </summary>
    /// <param name="trait"></param>
    /// <returns> The result of the roll </returns>
    public int rollTest(int trait) 
    {
        return DiceRoll.rollTest(trait);
    } 

    public int takeDamage(int damage, bool isTrueDamage = false)
    {

        if (roundManager == null) { roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>(); }
        if (sprite == null) { sprite = GetComponent<Image>(); }

        int actualDamage = isTrueDamage? damage : damage - def;


        if (actualDamage > 0)
        {
            hp -= actualDamage;

            if (hp <= 0)
            {
                die();
                roundManager.clashQueue.actionQueue.Clear(); // clears next actions on action queue since 
            }
            
         

            // set the crack intensity based on the percentage of hp left
            crackingSpriteOverlay.material.SetFloat("_Health", 1 -((float)hp / (float)maxHP) );

            StartCoroutine(FlashSprite(sprite, Color.red));
            StartCoroutine(MySceneManager.Instance.doPopUp(actualDamage.ToString(), transform.position, Color.red));

        
        }
        else
        {
            actualDamage = 0;

            StartCoroutine(FlashSprite(sprite, Color.white));
            StartCoroutine(MySceneManager.Instance.doPopUp(actualDamage.ToString(), transform.position, Color.grey));

        
        }


        return actualDamage; //return the actual damage taken 


    }

    public void takeTrueDamage(int damage) {
        takeDamage(damage, true);
    }

    public void heal(int value) {
        hp += value;
        
        if (hp > maxHP) {hp = maxHP;}

        StartCoroutine(MySceneManager.Instance.doPopUp(value.ToString(), this.transform.position, Color.green));
    }

    public void changeDEF(int value) {

        //OVERRIDE CHECK
        StatusEffect BlockerOverideEffectGainDEF = this.CheckForOverideInStatusEffects(OverideEffectType.GAIN_DEF);
        if (BlockerOverideEffectGainDEF != null && value > 0)
        {
            BlockerOverideEffectGainDEF?.overideEffectAct?.Invoke(new object[] {  }); // Invoke the override action if it exists
           return;
        }

        def += value;
    }

    public void changeMP(int value)
    {
        mp += value;
        if (mp > maxMP) { mp = maxMP; }
        if (mp < 0) { mp = 0; return;}
        

        if (value > 0) StartCoroutine(MySceneManager.Instance.doPopUp(value.ToString(), this.transform.position, Color.blue));
    }

    public void SetDEF(int value) {
        def = value;
    }

    public void AddSkill(Skill skill) {
        skills.Add(skill);
    }

    public void RemoveSkill(Skill skill) {
        skills.Remove(skill);
    }

    public void die() {

        isDead = true;
        //spriteRenderer.color = Color.black; //change the color of the sprite to gray
        Debug.Log(entityName + " has died.");

        // If player Dies
        if (this.entityType == EntityType.Player)
        {
            ButtonManager.Instance.closeAllMenus();

            roundManager.actionQueue.actionQueue.Clear(); // clears next actions on action queue
            roundManager.clashQueue.actionQueue.Clear();
            roundManager.entities.Remove(roundManager.player);
            roundManager.allies.Where(x => x != roundManager.player).ToArray();
            

            PlayerData.Instance.resetLevelsBeat(); // resets current level beaten, to start new death loop
            PlayerData.Instance.addFablePoints(fableWorth);
            PlayerData.Instance.incrementDeathCounter();
            StatusHudManager.Instance.updateLivesCounterUI();

            audioManager.ambienceOutput.Pause();
            audioManager.PlaySound(audioManager.death_sound); 

            items.Clear();
            PlayerData.Instance.changeIlhas( - PlayerData.Instance.getIlhas());

            // if player has no more lives
            if ((PlayerData.Instance.getLives() - PlayerData.Instance.getDeathCounter()) <= 0)
            {
                PlayerData.Instance.resetPlayerStatus();

                

                StartCoroutine(GameObject.FindObjectOfType<MySceneManager>().openSceneWithTransition(MySceneManager.SceneType.MAINMENU));
            }
            else 
            {
                //goes to fable shop on player death
                StartCoroutine(GameObject.FindObjectOfType<MySceneManager>().openSceneWithTransition(MySceneManager.SceneType.DEATHSHOP));
            }
            


        }
        else
        { // if not player



            //removes the dead entity from theirs respective arrays
            roundManager.entities = roundManager.entities.Where(x => x != this).ToList();
            GetComponent<Image>().raycastTarget = false; //makes the sprite not targetable anymore
            GetComponent<PolygonCollider2D>().enabled = false;
            

            if (this.entityType == EntityType.Enemy)
            {
                roundManager.enemiesKilled.Add(this);
                roundManager.enemies.Remove(this);

                CombatSetter.Instance.availableSpots.Add(spawnPoint);
                PlayerData.Instance.incrementKillCounter();


                GetComponent<Brain>().DyingAction();

            }
            else
            { //is an ally
                roundManager.allies = roundManager.allies.Where(x => x != this).ToList();
            }

            //cahnge into dissolve mateiral
            sprite.material = MaterialPallet.Instance.getColoredMaterial(
            MaterialPallet.Instance.getOriginColor(entityOrigin), 
            MaterialPallet.Instance.dissolveMaterial
            );

            crackingSpriteOverlay.enabled = false;
            StartCoroutine(AnimationManager.Instance.DissolveUponDeath(sprite)); //dissolves the entity upon death


            ActiveEffectManager.RemoveAllEffects(activeStatusEffects);

        
        }
        

    }

    /// <summary>
    /// Checks if the entity has a specific status effect based of SE_ID. Returns true if it does, false if it doesn't. If the input is null, returns false.
    /// </summary>
    public bool hasEffect(StatusEffect statusEffect) {
        if (statusEffect == null) { return false; }

        foreach (StatusEffect effect in activeStatusEffects)
        {
            if (effect.statusEffectID == statusEffect.statusEffectID)
            {
                return true; // Return true if the effect is found
            }
        }
        return false; // Return false if the effect is not found
    }

    public void removeEffect(string SE_ID) {
       
        foreach (StatusEffect effect in activeStatusEffects.ToList())
        {
            if (effect.statusEffectID == SE_ID)
            {
                ActiveEffectManager.Instance.KillEffect(effect); // Call the KillEffect method to remove the effect
                continue; // Exit the loop after removing the effect
            }
        }
        

    }


    public void resetActions()
    {
        currentMainActions = totalMainActions;
        currentSupActions = totalSupActions;
    }



    //flash the entity a inputed color
    public IEnumerator FlashSprite(Image sprite, Color color, float duration = 0.2f)
    {
        // Get the original material and set the new material
        Material originalMaterial = GetComponent<Image>().material;
        originalMaterial = GetComponent<Image>().material = originalMaterial;

        sprite.material = Resources.Load<Material>("Materials/whiteMaterial");

        // Set the color to the new color
        Color original = sprite.color;
        sprite.color = color;

        yield return new WaitForSeconds(duration);

        // Reset the material and color to original
        sprite.material = originalMaterial;
        sprite.color = original;

    }



    ////////////////////////////////////// Getters and Setters for HP, MP, Max HP, and Max MP ////////////////////////////////////////////////////


    public int getHP()
    {
        return hp;
    }
    public int getMP() {
        return mp;
    }
    public int getMaxHP() {
        return maxHP;
    }
    public int getMaxMP() {
        return maxMP;
    }
    public int getDEF() {
        return def;
    }
    public void setHP(int x) {
        hp = x;
    }
    public void setMP(int x) {
        mp = x;
    }
    
    public void setMaxHP(int x) {
        maxHP = x;
    }
    public void setMaxMP(int x) {
        maxMP = x;
        
    }
    
    public string getStatusAsString(){
        string stts = "";

        stts += "Name: " + entityName + "\n \n";
        stts +="HP: "+ hp + "/" +maxHP + "\n";
        stts +="MP " +mp + "/" +maxMP + "\n\n";
        stts +="DEF: "+ def +"\n";
        stts += "Base ATK: " +baseATK.ToString() + "\n";
        stts += "Current ATK: " + currentATK.ToString() + "\n";
        stts += "Current ATK Advantages: " + atkAdvantage + "\n \n";
        stts += "DEXTERITY: " + DEX + "\n";
        stts += "ATHLEtics: " + ATLETISM + "\n";

        stts += "Has Supporting Action = " +currentSupActions + "\n";



        return stts;

    }


    public StatusEffect CheckForOverideInStatusEffects(OverideEffectType overideEffectType) {
        foreach (StatusEffect effect in activeStatusEffects)
        {
            if (effect.overideEffectType == overideEffectType)
            {
                return effect;
            }
        }
        return null;
    }


    public void CopyFrom(Entity source)
{
    // Base Status
    entityType = source.entityType;
    entityOrigin = source.entityOrigin;
    entityName = source.entityName;
    entityID = source.entityID;

    hp = source.hp;
    maxHP = source.maxHP;
    mp = source.mp;
    maxMP = source.maxMP;
    def = source.def;

    // Traits
    DEX = source.DEX;
    ATLETISM = source.ATLETISM;
    AURA = source.AURA;
    CHARISM = source.CHARISM;
    LUCK = source.LUCK;
    INTUITION = source.INTUITION;
    HEX = source.HEX;
    INT = source.INT;
    WILL = source.WILL;
    REFLEX = source.REFLEX;
    PERSEPTION = source.PERSEPTION;
    FURTIVITY = source.FURTIVITY;
    CONSTITUTION = source.CONSTITUTION;
    DOMINANCE = source.DOMINANCE;

    // Attack
    baseATK = new DiceRoll(source.baseATK);
    currentATK = new DiceRoll(source.currentATK);
    atkAdvantage = source.atkAdvantage;

    // States
    isDead = source.isDead;
    totalMainActions = source.totalMainActions;
    totalSupActions = source.totalSupActions;
    currentMainActions = source.currentMainActions;
    currentSupActions = source.currentSupActions;

    // Skills
    skills = source.skills
    .Select(skill => Instantiate(skill))
    .ToList();

    // Status Effects
    activeStatusEffects = source.activeStatusEffects
    .Select(effect => Instantiate(effect))
    .ToList();

    // Items
    items = source.items
    .Select(item => Instantiate(item))
    .ToList();

    // Fable
    fableWorth = source.fableWorth;
}

}
