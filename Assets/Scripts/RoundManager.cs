using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class RoundManager : MonoBehaviour
{
    public SkillManager skillManager;
    public ButtonManager buttonManager;

    [SerializeField] public GameObject damagePopupPrefab;
    [SerializeField] public Transform hud; //the parent object for the damage popups
    [SerializeField] public ActionQueue actionQueue;
    public GameObject act_menu;

    public Entity currentTurn;
    public Entity target;
    public Skill skillSelected;
    public TurnPhase currentPhase; //the current phase of the turn

    public Entity player;
    public Entity[] entities; //array of all entities in the scene
    public Entity[] allies; //array of all allies in the scene
    public Entity[] enemies; //array of all enemies in the scene

    public bool test = false; //for testing purposes

    
    [SerializeField] public enum TurnPhase
    {
        Start,
        Action,
        targetingATK,
        targetingSKILL,
        targetingITEM,
        End
    }



    public void Start()
    {
        

        entities = FindObjectsOfType<Entity>(true); //finds all entities in the scene

        foreach (Entity entity in entities) //loops through each entity
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
        
        //sorts the entities by their rolls in descending order
        entities = entities.OrderByDescending(x => x.rollDEX()).ToArray();

        currentTurn = entities[0]; //sets the current turn to the first entity in the array
        
        //set act_menu to active if the current turn is the player's
        act_menu.SetActive(currentTurn.entityType == Entity.EntityType.Player);

        actionQueue.Enqueue("delay" , () => Delay(1f));
        actionQueue.Enqueue("StartTurn", () => StartTurn());
        
    }

    public void Update()
    {
        test = act_menu.activeInHierarchy; //for testing purposes
    }

    //endturn picks the next entity to take their turn
    public void EndTurn(){ actionQueue.Enqueue("EndTurn", () => EndTurnCoroutine());}
    public IEnumerator EndTurnCoroutine(){

        currentPhase = TurnPhase.End; //set the current phase to end

        // set the next turn to the next entity in the array, or loop back to the first entity if at the end
        int index = System.Array.IndexOf(entities, currentTurn);
        currentTurn = (index == entities.Length - 1) ? entities[0] : entities[index + 1];
        
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
        
        currentTurn.hasSupAction = true; //set the hasSupAction to true for the current turn
        
        if (currentTurn.entityType == Entity.EntityType.Enemy)
        {


            yield return Delay(1f); 
            currentPhase = TurnPhase.Action; //set the current phase to action
            target = allies[UnityEngine.Random.Range(0, allies.Length)];
            yield return StartCoroutine(currentTurn.doBasicATK(target));
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


        foreach (var enemy in enemies)
        {
            var enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
                enemyCollider.enabled = enable;

            var enemyRender = enemy.GetComponent<SpriteRenderer>();
            if (enemyRender != null)
                enemyRender.color = enable ? new Color(1f, 0f, 0f) : Color.white;
        }
    }

    public void EnableSkillTargetingUI(bool enable)
    {


        foreach (var entity in entities)
        {
            var enemyCollider = entity.GetComponent<Collider2D>();
            if (enemyCollider != null)
                enemyCollider.enabled = enable;

            var enemyRender = entity.GetComponent<SpriteRenderer>();
            if (enemyRender != null)
                enemyRender.color = enable ? new Color(0f, 0f, 1f) : Color.white;
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

            actionQueue.Enqueue("PlayerAttack", () => currentTurn.doBasicATK(target));
        }
        else if (currentPhase == TurnPhase.targetingSKILL) 
        {
            EnableSkillTargetingUI(false);
            buttonManager.skillMenu.SetActive(false);
            act_menu.SetActive(false); 

            buttonManager.inSkillOverlay = false;

            skillManager.doSkill(target , currentTurn, skillSelected);

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
