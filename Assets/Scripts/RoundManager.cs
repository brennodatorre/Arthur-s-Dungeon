
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;

using UnityEngine;
using UnityEngine.Video;

using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Unity.VisualScripting;


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
    

    [Space(5)]
    public Entity currentTurn;
    public Entity target;
    public Skill skillSelected;
    public Item itemSelected;
    public TurnPhase currentPhase; //the current phase of the turn

    [Space(10)]
    public List<Entity> entities; //array of all entities in the scene
    public Entity[] allies; //array of all allies in the scene
    public Entity[] enemies; //array of all enemies in the scene

    [Space(10)]
    [Header ("Player Info")]
    public Entity player;
    public bool playerIsAttacking = false; //if the player is currently attacking
    public bool playerIsTargeting;
    public bool playerCanAct;
    public int numberOfRounds;



    [HideInInspector] public bool combatIsDone = false;



    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }

        
    }


    private void Update()
    {

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
    }



    public void Start()
    {

        matPallet = MaterialPallet.Instance;

        combatIsDone = false;
        numberOfRounds = 0;

        StatusHudManager.Instance.updateLivesCounterUI();
        StatusHudManager.Instance.updateLevelCounterUI();

        PlayerData.Instance.LoadPlayerData(player); // load player data 

        combatSetter.openLevel(); // sets the enemies level

        entities = FindObjectsOfType<Entity>(true).ToList(); //finds all entities in the scene and converts to a list

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

    }



    private IEnumerator StartTurn()
    {
        currentPhase = TurnPhase.Start; //set the current phase to start

        yield return null; //wait for the end of the frame to ensure all actions are processed
        
        currentTurn.resetActions(); //reset the actions for the current turn
        
        if (currentTurn.entityType == Entity.EntityType.Enemy)
        {


            yield return Delay(1f); 
            currentPhase = TurnPhase.Action; //set the current phase to action
            target = allies[UnityEngine.Random.Range(0, allies.Length)];
            actionQueue.Enqueue("EnemyAttack", () => currentTurn.doBasicAtkCaller(target));
            
        }
        else if (currentTurn.entityType == Entity.EntityType.NPC)
        {

            currentPhase = TurnPhase.Action; //set the current phase to action
            Debug.Log("NPC turn");
            yield return Delay(1f); 
            
        }
        
        else if (currentTurn.entityType == Entity.EntityType.Player)
        {

            actionQueue.Enqueue("delay" , () => Delay(1f));
            act_menu.SetActive(true);
            currentPhase = TurnPhase.Action; //set the current phase to action
            

            
            
        }
        else
        {
            Debug.Log("Unknown entity type: " + currentTurn.entityType); 
        }
    }

    //endturn picks the next entity to take their turn
    public void EndTurn() { actionQueue.Enqueue("EndTurn", () => EndTurnCoroutine()); }
    public IEnumerator EndTurnCoroutine(){

        
        currentPhase = TurnPhase.End; //set the current phase to end

        yield return null; //wait for the end of the frame to ensure all actions are processed

        // set the next turn to the next entity in the array, or loop back to the first entity if at the end
        int index = entities.IndexOf(currentTurn);
        currentTurn = (index == entities.Count - 1) ? entities[0] : entities[index + 1];
        
        if (currentTurn.entityType != Entity.EntityType.Player) {yield return Delay(1f); }//wait for 1 second before starting the next turn

        foreach (Entity entity in entities) 
        {
            skillManager.resetSkills(entity); //reset the skills for the next turn
        }

        numberOfRounds++;

        actionQueue.Enqueue("StartTurn", () => StartTurn());
    }






    public void EnableEnemyTargetingUI(bool enable)
    {

        player.GetComponent<Collider2D>().enabled = !enable; //does not allow player self hit

        foreach (var enemy in enemies) // for each enemy
        {
            Image enemyRender = enemy.GetComponent<Image>();

            var enemyCollider = enemy.GetComponent<Collider2D>();

            if (enemyCollider != null) { enemyCollider.enabled = enable; } // enable collider
            if (enemyRender != null && !enemy.isDead)//sets color
            {
                enemyRender.material = enable ?
                    matPallet.getColoredMaterial(matPallet.red, matPallet.outlineSpriteMaterial) : //red outline if atk targetting
                    matPallet.getColoredMaterial(matPallet.getEntityOriginColor(enemy), matPallet.dissolveMaterial); // back to normal dissolve matrial
            } 
        }


    }

    public void toggleEntityTargetingUI(bool enable)
    {


        foreach (var entity in entities)
        {
            //tell their paht object that they can start being targetted if the skill used is paht
            if (skillSelected != null && skillSelected.isPAHTSkill && buttonManager.inSkillOverlay) entity.GetComponent<PressAndHoldTarget>().isWaiting = enable;
            else if (itemSelected != null && itemSelected.isPAHTItem && buttonManager.inItemOverlay) entity.GetComponent<PressAndHoldTarget>().isWaiting = enable;

            var entityColiider = entity.GetComponent<Collider2D>();
            Image entityRender = entity.GetComponent<Image>();

            if (entityColiider != null && entity.entityType == Entity.EntityType.Enemy)
                entityColiider.enabled = enable;

            if (entityRender != null && !entity.isDead)//sets color
            {
                entityRender.material = enable ?
                    matPallet.getColoredMaterial(matPallet.blue, matPallet.outlineSpriteMaterial) : //blue outline if atk targetting
                    matPallet.getColoredMaterial(matPallet.getEntityOriginColor(entity), matPallet.dissolveMaterial); // back to normal dissolve matrial
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

    public IEnumerator ShowDamagePopup(float damage, Vector3 position, Color color)
    {
        yield return new WaitForSeconds(0.2f); // delay before showing the popup

        GameObject popup = Instantiate(damagePopupPrefab, hud);
        popup.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
        popup.GetComponentInChildren<TextMeshProUGUI>().color = color;
        popup.transform.position = position;
        
    }
    private IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }




}
