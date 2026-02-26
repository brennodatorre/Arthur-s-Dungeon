
using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;

using Image = UnityEngine.UI.Image;

[System.Serializable]
public class Entity : MonoBehaviour
{
    private RoundManager roundManager;
    private Image sprite;
    private LogManager logManager; 
    private AudioManager audioManager;
    public Brain brain;

    public enum EntityType { Player, Enemy, NPC };
    public enum EntityOrigin {ROSES, HEX, LANDREAS, ARTHUR, SYSTEM, UNKNOWN, SURVIVOR, FLAME }


    public GameObject statEffectDisplay;


    [Space(5)]
    [Header("Base Status")]
    [SerializeField] public EntityType entityType;
    public EntityOrigin entityOrigin;
    [SerializeField] public new string name;
    [SerializeField] private int hp;
    [SerializeField] private int maxHP;
    [SerializeField] private int mp;
    [SerializeField] private int maxMP;
    [SerializeField] public int def;




    public enum Trait { DEX, ATLETISM, AURA, CHARISM, LUCK, INTUITION, HEX, INT, WILL, REFLEX, PERSEPTION, FURTIVIDY, CONSTITUTION, DOMINANCE };
    [Space(10)]
    [Header("Traits")]
    

    [SerializeField] public int DEX = 1;
    [SerializeField] public int ATLETISM = 1;
    [SerializeField] public int AURA = 1;
    [SerializeField] public int CHARISM = 1;
    [SerializeField] public int LUCK = 1;
    [SerializeField] public int INTUITION = 1;
    [SerializeField] public int HEX = 1;
    [SerializeField] public int INT = 1;
    [SerializeField] public int WILL = 1;
    [SerializeField] public int REFLEX = 1;
    [SerializeField] public int PERSEPTION = 1;
    [SerializeField] public int FURTIVITY = 1;
    [SerializeField] public int CONSTITUTION = 1;
    [SerializeField] public int DOMINANCE = 1;


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
    [SerializeField] public List<StatusEffect> activeSkillEffects = new List<StatusEffect>();


    [Space(10)]
    [Header("Items")]
    [SerializeField] public List<Item> items = new List<Item>(); // to not edit original copy

    [Space(10)]
    [Header("Fable Status")]
    public int fableWorth;

    
    


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

        roundManager = GameObject.Find("CombatManager").GetComponent<RoundManager>();
        sprite = GetComponent<Image>();
        logManager = FindObjectOfType<LogManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();


        sprite.material = MaterialPallet.Instance.getColoredMaterial(MaterialPallet.Instance.getEntityOriginColor(this), MaterialPallet.Instance.dissolveMaterial);

        if (baseATK.dices.Count == 0) { baseATK.AddDice(0, 0); }

        //copies baseATK dices to currentATK
        foreach (Dice dice in baseATK.dices)
        {
            this.currentATK.AddDice(dice.count, dice.sides);
        }
        currentATK.AddModifier(baseATK.getModifier());

        
        
        if (entityType != EntityType.Player) {brain = GetComponent<Brain>(); }

        
       
    }

    public IEnumerator doBasicAtkCaller(Entity target)
    {
        roundManager.currentPhase = RoundManager.TurnPhase.Clash;

        yield return new WaitForSeconds(0f);

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

    //rolls a test based on inputed trait
    public int rollTest(Trait trait)
    {
        int result = 0;

        switch (trait)
        {
            case Trait.DEX:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(DEX - 1);
                break;
            case Trait.ATLETISM:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(ATLETISM - 1);
                break;
            case Trait.AURA:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(AURA - 1);
                break;
            case Trait.CHARISM:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(CHARISM - 1);
                break;
            case Trait.LUCK:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(LUCK - 1);
                break;
            case Trait.INTUITION:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(INTUITION - 1);
                break;
            case Trait.HEX:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(HEX - 1);
                break;
            case Trait.INT:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(INT - 1);
                break;
            case Trait.WILL:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(WILL - 1);
                break;
            case Trait.REFLEX:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(REFLEX - 1);
                break;
            case Trait.PERSEPTION:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(PERSEPTION - 1);
                break;
            case Trait.FURTIVIDY:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(FURTIVITY - 1);
                break;
            case Trait.CONSTITUTION:
                result = new DiceRoll(new List<Dice> { new Dice(1, 20) }, 0).Roll(CONSTITUTION - 1);
                break;
        }

        return result;

    } 

    public int takeDamage(int damage)
    {

        if (roundManager == null) { roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>(); }
        if (sprite == null) { sprite = GetComponent<Image>(); }

        int actualDamage = damage - def;
        if (actualDamage > 0)
        {
            hp -= damage - def;

            if (hp <= 0)
            {
                die();
                roundManager.clashQueue.actionQueue.Clear(); // clears next actions on action queue since 
            }
            else
            {
                // Flash red to indicate damage taken
                roundManager.clashQueue.Enqueue("FlashRed", () => FlashSprite(sprite, Color.red));

                //show damage popup
                roundManager.clashQueue.Enqueue("showDamagePopup", () => MySceneManager.Instance.doPopUp(actualDamage.ToString(), transform.position, Color.red));
            }
        }
        else
        {
            actualDamage = 0;

            // Flash White to indicate block
            roundManager.clashQueue.Enqueue("FlashWhite", () => FlashSprite(sprite, Color.white));

            //show damage popup
            roundManager.clashQueue.Enqueue("showDamagePopup", () => MySceneManager.Instance.doPopUp(actualDamage.ToString(), transform.position, Color.gray));
        }


        return actualDamage; //return the actual damage taken 


    }

    public void takeTrueDamage(int damage) {
        hp -=  damage;
        

        if (hp <= 0) { die(); }
        else
        {

            // Flash red to indicate damage taken
            roundManager.clashQueue.Enqueue("FlashRed", () => FlashSprite(sprite, Color.red));

            //show damage popup
            roundManager.clashQueue.Enqueue("showDamagePopup", () => MySceneManager.Instance.doPopUp(damage.ToString(), transform.position, Color.red));
        }
    }

    public void heal(int value) {
        hp += value;
        
        if (hp > maxHP) {hp = maxHP;}

        StartCoroutine(MySceneManager.Instance.doPopUp(value.ToString(), this.transform.position, Color.green));
    }

    

    public void changeMP(int value)
    {
        mp += value;
        if (mp > maxMP) { mp = maxMP; }
        if (mp < 0) { mp = 0; return;}
        

        if (value > 0) StartCoroutine(MySceneManager.Instance.doPopUp(value.ToString(), this.transform.position, Color.blue));
    }

    public void SetDef(int value) {
        def += value;
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
        Debug.Log(name + " has died.");

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

            // if player has no more lives
            if ((PlayerData.Instance.getLives() - PlayerData.Instance.getDeathCounter()) <= 0)
            {
                PlayerData.Instance.resetPlayerStatus();

                StartCoroutine(GameObject.FindObjectOfType<MySceneManager>().openSceneWithTransition(MySceneManager.SceneType.TUTORIAL));
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

            if (this.entityType == EntityType.Enemy)
            {

                roundManager.enemies = roundManager.enemies.Where(x => x != this).ToArray();

                PlayerData.Instance.addFablePoints(fableWorth);
                PlayerData.Instance.incrementKillCounter();
                PlayerData.Instance.changeIlhas(2);

            }
            else
            { //is an ally
                roundManager.allies = roundManager.allies.Where(x => x != this).ToArray();
            }


            StartCoroutine(AnimationManager.Instance.DissolveUponDeath(sprite)); //dissolves the entity upon death

            // ///// fix this for mult enemy damage, it will trigger that many times if u end the combat by killing more than one enemy \\\\\\
            // if (roundManager.enemies.Length == 0 && !roundManager.combatIsDone)
            // {  //goes to next combat level

            //     PlayerData.Instance.incrementLevelsBeat();
            //     doInBtwnLevelPlayerRegen();
            //     PlayerData.Instance.savePlayerData(roundManager.player);

            //     StatusHudManager.Instance.updateLevelCounterUI();

            //     roundManager.combatIsDone = true;



            //     StartCoroutine(MySceneManager.Instance.openSceneWithTransition("COMBAT", false));
            // }

        
        
        }
        

    }

    public bool hasEffect(StatusEffect statusEffect) {
        if (statusEffect == null) { return false; }

        foreach (StatusEffect effect in activeSkillEffects)
        {
            if (effect.effectName == statusEffect.effectName)
            {
                return true; // Return true if the effect is found
            }
        }
        return false; // Return false if the effect is not found
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

        stts += "Name: " + name + "\n \n";
        stts +="HP: "+ hp + "/" +maxHP + "\n";
        stts +="MP " +mp + "/" +maxMP + "\n\n";
        stts +="DEF: "+ def +"\n";
        stts += "Base ATK: " +baseATK.diceToString() + "\n";
        stts += "Current ATK: " + currentATK.diceToString() + "\n";
        stts += "Current ATK Advantages: " + atkAdvantage + "\n \n";
        stts += "DEXTERITY: " + DEX + "\n";
        stts += "ATHLEtics: " + ATLETISM + "\n";

        stts += "Has Supporting Action = " +currentSupActions + "\n";



        return stts;

    }

}
