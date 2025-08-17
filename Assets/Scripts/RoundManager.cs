
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;


[System.Serializable]
public class RoundManager : MonoBehaviour
{

    public static RoundManager Instance;

    public SkillManager skillManager;
    public ButtonManager buttonManager;
    public AnimationManager animationManager;
    public CombatSetter combatSetter;


     [SerializeField] public enum TurnPhase
    {
        Start,
        Action,
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

    public Entity currentTurn;
    public Entity target;
    public Skill skillSelected;
    public TurnPhase currentPhase; //the current phase of the turn
    
    [Space]
    public Entity player;
    public bool playerIsAttacking = false; //if the player is currently attacking

    [Space]
    public List<Entity> entities; //array of all entities in the scene
    public Entity[] allies; //array of all allies in the scene
    public Entity[] enemies; //array of all enemies in the scene



    
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



    public void Start()
    {
        
        PlayerData.Instance.LoadPlayerData(player); // load player data 

        combatSetter.openLevel(); // sets the enemies level

        entities = FindObjectsOfType<Entity>(true).ToList(); //finds all entities in the scene and converts to a list

        foreach (Entity entity in entities.ToArray()) //loops through each entity
        {
            if (entity.isActiveAndEnabled){
                if (entity.entityType != Entity.EntityType.Enemy) //if the entity is not an enemy
                {
                    allies = allies.Append(entity).ToArray(); //adds the entity to the allies array
                }
                else if (entity.entityType == Entity.EntityType.Enemy) //if the entity is an enemy
                {
                    enemies = enemies.Append(entity).ToArray(); //adds the entity to the enemies array
                }
            }
            else {entities.Remove(entity);} //removes the entity from the list
        }
        
        //sorts the entities by their rolls in descending order
        entities = entities.OrderByDescending(x => x.rollDEX()).ToList();

        currentTurn = entities[0]; //sets the current turn to the first entity in the array
        
        //sets all menus off so the combat can begin
        act_menu.SetActive(false);
        buttonManager.skillMenu.SetActive(false);
        buttonManager.itemMenu.SetActive(false);

        actionQueue.Enqueue("delay" , () => Delay(2f));
        actionQueue.Enqueue("StartTurn", () => StartTurn());
        
    }

    //endturn picks the next entity to take their turn
    public void EndTurn() { actionQueue.Enqueue("EndTurn", () => EndTurnCoroutine());}
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

 
    public void EnableEnemyTargetingUI(bool enable)
    {

        player.GetComponent<Collider2D>().enabled = !enable; //does not allow player self hit

        foreach (var enemy in enemies)
        {
            var enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
                enemyCollider.enabled = enable;

            var enemyRender = enemy.GetComponent<SpriteRenderer>();
            if (enemyRender != null && !enemy.isDead)
                enemyRender.color = enable ? new Color(1f, 0f, 0f) : Color.white;
        }

        // player.GetComponent<Collider2D>().enabled = enable; //does not allow player self hit
    }

    public void EnableSkillTargetingUI(bool enable)
    {


        foreach (var entity in entities)
        {
            var entityColiider = entity.GetComponent<Collider2D>();
            if (entityColiider != null && entity.entityType == Entity.EntityType.Enemy)
                entityColiider.enabled = enable;

            var entityRender = entity.GetComponent<SpriteRenderer>();
            if (entityRender != null  && !entity.isDead)
                entityRender.color = enable ? new Color(0f, 0f, 1f) : Color.white;
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
            EnableSkillTargetingUI(false);

            //unlock the skill buttons in case there are more skills to use
            buttonManager.toggleBtns(true, buttonManager.skillButtons); 

            buttonManager.skillMenu.SetActive(false);
            act_menu.SetActive(false);

            buttonManager.inSkillOverlay = false;

            skillManager.doSkill(target, currentTurn, skillSelected);

        }
        // else if (currentPhase == TurnPhase.targetingITEM) 
        // {
        //     EnableItemTargetingUI(false);
        //     act_menu.SetActive(false);
        //    
        // }
       
        
    
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
