
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UI;





[System.Serializable]
public class RoundManager : MonoBehaviour
{

    public static RoundManager Instance;

    public SkillManager skillManager;
    public ItemManager itemManager;
    public ButtonManager buttonManager;
    public AnimationManager animationManager;
    public CombatSetter combatSetter;
    public MaterialPallet matPallet;


    [SerializeField] public enum TurnPhase
    {
        Start,
        Action,
        Clash,
        targetingATK,
        targetingSKILL,
        targetingITEM,
        End
    }

    

    [SerializeField] public GameObject damagePopupPrefab;
    [SerializeField] public Transform hud; //the parent object for the damage popups
    [SerializeField] public ActionQueue actionQueue;
    [SerializeField] public ActionQueue clashQueue;
    public GameObject act_menu;
    public GameObject lootScreen;

    [Space(5)]
    [Header ("Loot Menu")]
    public Sprite inBtwnLevelIcon;
    public Sprite goldIcon;
    public Sprite fableIcon;
    public AudioClip coinSound;
    public AudioClip healSound;
    public AudioClip fableSound;

    

    [Space(10)]
    public Entity currentTurn;
    public Entity target;
    public Skill skillSelected;
    public Item itemSelected;
    public TurnPhase currentPhase; //the current phase of the turn

    [Space(10)]
    public List<Entity> entities; //array of all entities in the scene
    public Entity[] allies; //array of all allies in the scene
    public Entity[] enemies; //array of all enemies in the scene
    public List<Entity> enemiesKilled = new List<Entity>(); 

    [Space(10)]
    [Header ("Player Info")]
    public Entity player;
    public bool playerIsAttacking = false; //if the player is currently attacking
    public bool playerIsTargeting;
    public bool playerCanAct;
    public int numberOfRounds;
    private bool setupWasdone = false;
    private bool lootMenuIsUp = false;






    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }

        
    }


    private void Update()
    {
         if (!setupWasdone) {StartSetup(); }

        playerIsTargeting =
            currentPhase == RoundManager.TurnPhase.targetingATK ||
            currentPhase == RoundManager.TurnPhase.targetingSKILL||
            currentPhase == RoundManager.TurnPhase.targetingITEM 
        ;

        playerCanAct =
            !player.isDead &&
            currentTurn == player &&                              //on player's turn and
            currentTurn.currentMainActions > 0 &&                 //player has Main-Aactions and
            (currentPhase == RoundManager.TurnPhase.Action ||     //(player is on action menu or
            playerIsTargeting)                                    // is targetting)
        ;

        //ends the comabt 
        if (enemies.Length == 0 && !lootMenuIsUp)
        { 

            lootMenuIsUp = true;

            PlayerData.Instance.incrementLevelsBeat();
            StatusHudManager.Instance.updateLevelCounterUI();

            GameObject healItem = null;
            GameObject goldItem = null;
            GameObject fableItem = null;
            

            healItem = lootScreen.GetComponentInChildren<MenuContainerManager>().AddItem(inBtwnLevelIcon, "Heal", 
                () => {
                     doInBtwnLevelPlayerRegen();
                     AudioManager.Instance.PlaySound(healSound);
                     lootScreen.GetComponentInChildren<MenuContainerManager>().removeItem(healItem);

                });

            goldItem = lootScreen.GetComponentInChildren<MenuContainerManager>().AddItem(goldIcon, "Gold", 
                () => {
                    int gain = 2 * enemiesKilled.Count;
                    PlayerData.Instance.changeIlhas(gain);
                    AudioManager.Instance.PlaySound(coinSound);
                    StartCoroutine(MySceneManager.Instance.doPopUp(gain.ToString(), this.transform.position, Color.yellow));
                    lootScreen.GetComponentInChildren<MenuContainerManager>().removeItem(goldItem);
                });

            fableItem = lootScreen.GetComponentInChildren<MenuContainerManager>().AddItem(fableIcon, "Fable Points", 
                () => {
                    int value = 0;
                    foreach (var enemy in enemiesKilled) { value += enemy.fableWorth; }
                    PlayerData.Instance.addFablePoints(value);
                    AudioManager.Instance.PlaySound(fableSound);
                    StartCoroutine(MySceneManager.Instance.doPopUp(value.ToString(), this.transform.position, Color.magenta));
                    lootScreen.GetComponentInChildren<MenuContainerManager>().removeItem(fableItem);

                    });


            lootScreen.SetActive(true);
            


            
            
        }

    }



    private void StartSetup()
    {

        matPallet = MaterialPallet.Instance;

        
        numberOfRounds = 0;

        StatusHudManager.Instance.updateLivesCounterUI();
        StatusHudManager.Instance.updateLevelCounterUI();

        PlayerData.Instance.LoadPlayerData(player); // load player data 

        combatSetter.openLevel(); // sets the enemies level

        entities = FindObjectsOfType<Entity>(true).Where(e => !e.IsAClass).ToList(); //finds all entities in the scene and converts to a list

        foreach (Entity entity in entities.ToArray()) //loops through each entity
        {
            if (entity.isActiveAndEnabled)
            {
                if (entity.entityType != Entity.EntityType.Enemy) //if the entity is not an enemy
                {
                    allies = allies.Append(entity).ToArray(); //adds the entity to the allies array
                }
                else if (entity.entityType == Entity.EntityType.Enemy) //if the entity is an enemy
                {
                    enemies = enemies.Append(entity).ToArray(); //adds the entity to the enemies array
                }
            }
            else { entities.Remove(entity); } //removes the entity from the list
        }

        //sorts the entities by their rolls in descending order
        entities = entities.OrderByDescending(x => x.rollDEX()).ToList();

        currentTurn = entities[0]; //sets the current turn to the first entity in the array

        //sets all menus off so the combat can begin
        act_menu.SetActive(false);
        buttonManager.skillMenu.SetActive(false);
        buttonManager.itemMenu.SetActive(false);

        actionQueue.Enqueue("delay", () => Delay(2f));
        actionQueue.Enqueue("StartTurn", () => StartTurn());

        setupWasdone =true;

    }



    private IEnumerator StartTurn()
    {
        StatusEffect stunnedSE = ActiveEffectManager.Instance.statusEffectPrefabs.StunnedEffect;

        //calculates the intent of each entity that is not the start of the round
        if (currentTurn == entities[0])
        {
            foreach (Entity entity in entities)
            {
                if (entity.entityType == Entity.EntityType.Player) {continue;}

                if (!entity.hasEffect(stunnedSE)) entity.GetComponent<Brain>().getIntent();
            }
        }

        currentPhase = TurnPhase.Start; //set the current phase to start

        yield return null; //wait for the end of the frame to ensure all actions are processed
        
        currentTurn.resetActions(); //reset the actions for the entity

        currentPhase = TurnPhase.Action; //set the current phase to action

        if (currentTurn.hasEffect(stunnedSE)) { EndTurn(); }

        else if (currentTurn.entityType == Entity.EntityType.Enemy)
        {

            yield return Delay(1f);             
            currentTurn.brain.doIntent(currentTurn, allies);
            
        }
        else if (currentTurn.entityType == Entity.EntityType.NPC)
        {

            Debug.Log("NPC turn");
            yield return Delay(1f); 
            
        }
        
        else if (currentTurn.entityType == Entity.EntityType.Player)
        {

            actionQueue.Enqueue("delay" , () => Delay(1f));
            act_menu.SetActive(true);
            
        }

    }

    //endturn picks the next entity to take their turn
    public void EndTurn() { actionQueue.Enqueue("EndTurn", () => EndTurnCoroutine()); }
    public IEnumerator EndTurnCoroutine(){

        
        currentPhase = TurnPhase.End; //set the current phase to end

        yield return null; //wait for the end of the frame to ensure all actions are processed

        currentTurn.GetComponent<Brain>()?.clearIntent(); //clear the intent of the current turn
        

        // set the next turn to the next entity in the array, or loop back to the first entity if at the end
        int index = entities.IndexOf(currentTurn);
        currentTurn = (index == entities.Count - 1) ? entities[0] : entities[index + 1];
        
        if (currentTurn.entityType != Entity.EntityType.Player) {yield return Delay(1f); }

        foreach (Entity entity in entities) 
        {
            skillManager.resetSkills(entity); //reset the skills for the next turn
        }

        numberOfRounds++;

        actionQueue.Enqueue("StartTurn", () => StartTurn());
    }



    #region TargetingUI


    public void EnableEnemyTargetingUI(bool enable)
    {

        player.GetComponent<Collider2D>().enabled = !enable; //does not allow player self hit

        foreach (var enemy in enemies) // for each enemy
        {
            Image enemyRender = enemy.GetComponent<Image>();

            if (enemyRender != null && !enemy.isDead)//sets color
            {
                if(enable) 
                    {enemyRender.material = matPallet.getColoredMaterial(matPallet.red, matPallet.outlineSpriteMaterial);} //red outline if atk targetting
                else {enemyRender.material = matPallet.getColoredMaterial(matPallet.getOriginColor(enemy.entityOrigin), matPallet.crackMaterial); // back to normal dissolve matrial
                    }
            } 
        }


    }

    public void toggleEntityTargetingUI(bool enable)
    {


        foreach (var entity in entities)
        {
            //tell their paht object that they can start being targetted if the skill used is paht
            if (skillSelected != null && skillSelected.isPAHTSkill && buttonManager.inSkillOverlay) entity.GetComponent<PressAndHoldTarget>().isWaiting = true;
            else if (itemSelected != null && itemSelected.isPAHTItem && buttonManager.inItemOverlay) entity.GetComponent<PressAndHoldTarget>().isWaiting = true;
            else { entity.GetComponent<PressAndHoldTarget>().isWaiting = false; }
            
            
            Image entityRender = entity.GetComponent<Image>();

            if (entityRender != null && !entity.isDead)//sets color
            {
                if (enable ) entityRender.material = matPallet.getColoredMaterial(matPallet.blue, matPallet.outlineSpriteMaterial) ; //blue outline if atk targetting
                else {entityRender.material = matPallet.getColoredMaterial(matPallet.getOriginColor(entity.entityOrigin), matPallet.crackMaterial); // back to normal dissolve matrial
                    }
            } 
        }
    }
    

    public void OnTargetSelected(Entity selected)
    {
        target = selected;


        if (currentPhase == TurnPhase.targetingATK)
        {
            EnableEnemyTargetingUI(false);
            buttonManager.toggleBtns(true, buttonManager.actionButtons); //turn on the action buttons locally(so they are on when players next turn starts)
            act_menu.SetActive(false);

            buttonManager.inAtkOverlay = false;

            playerIsAttacking = true; //set the player as attacking

            actionQueue.Enqueue("PlayerAttack", () => currentTurn.doBasicAtkCaller(target));

        }
        else if (currentPhase == TurnPhase.targetingSKILL)
        {

            toggleEntityTargetingUI(false);

            //unlock the skill buttons in case there are more skills to use
            buttonManager.unblockSkillButtons(buttonManager.skillButtons, buttonManager.lastButtonPressed);
            buttonManager.toggleBtns(true, buttonManager.skillButtons);

            buttonManager.skillMenu.SetActive(false);

            act_menu.SetActive(false);

            buttonManager.inSkillOverlay = false;

            if (skillSelected.isPAHTSkill) // if the skill is a paht 
            {
                // if paht was completed do the skill
                if (CursorManager.Instance.holdableMEM.askIfPAHTWasCompleted())
                {
                    skillManager.doSkill(target, currentTurn, skillSelected);
                    currentPhase = TurnPhase.Action;
                }
                else
                {
                    // else, open the skill menu
                    buttonManager.skillMenu.SetActive(true);
                }
            }
            else
            {
                skillManager.doSkill(target, currentTurn, skillSelected);
                currentPhase = TurnPhase.Action;
            }



        }
        else if (currentPhase == TurnPhase.targetingITEM)
        {
            toggleEntityTargetingUI(false);
            
            //unlock the skill buttons in case there are more skills to use
            buttonManager.unblockSkillButtons(buttonManager.itemButtons, buttonManager.lastButtonPressed);
            buttonManager.toggleBtns(true, buttonManager.itemButtons);

            buttonManager.itemMenu.SetActive(false);

            act_menu.SetActive(false);

            buttonManager.inItemOverlay = false;

            if (itemSelected.isPAHTItem) // if the item is a paht 
            {
                // if paht was completed use the item
                if (CursorManager.Instance.holdableMEM.askIfPAHTWasCompleted())
                {
                    itemManager.useItem(target, currentTurn, itemSelected);
                    currentPhase = TurnPhase.Action;
                }
                else
                {
                    // else, open the skill menu
                    buttonManager.itemMenu.SetActive(true);
                }
            }
            else
            {
                itemManager.useItem(target, currentTurn, itemSelected);
                currentPhase = TurnPhase.Action;
            }
            
           
        }



    }

    #endregion





    private IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }



    public void doInBtwnLevelPlayerRegen()
    {
        int h = Mathf.CeilToInt(player.getMaxHP() / 10f);
        int m = Mathf.CeilToInt(player.getMaxMP() / 10f);
        player.heal( h);
        player.changeMP( m );


    }


}
