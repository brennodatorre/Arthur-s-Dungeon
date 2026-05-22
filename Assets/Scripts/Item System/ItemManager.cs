using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [HideInInspector] public static ItemManager Instance;

    public RoundManager roundManager;
    public LogManager logManager;
    public AudioManager audioManager;
    public ActiveEffectManager activeEffectManager;

    public List<Item> items = new List<Item>();

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


    public void useItem( Entity target, Entity caster, Item item)
    {

        bool isTargettingEnemy = RoundManager.Instance.enemies.Contains(target);

        // deals with self targeting item (casting a self target into someone else)
        if (target != caster && item.targetType == Item.ItemTarget.Self)
        {
            audioManager.PlaySound(audioManager.skill_unable_sound);
            logManager.AddLog(item.itemName + " can only be used on yourself.");
            roundManager.buttonManager.closeItemMenu(); //close the skill menu after the action is done
            return;
        }



        if (item.CanBeUsed(caster, target, item))
        {
            //use up the sup action if the skill is a support action
            if (item.actionType == Item.ItemActionType.Sup) { caster.currentSupActions--; }
            else if (item.actionType == Item.ItemActionType.Main) { caster.currentMainActions--; }

            //deals with the skill's fulldescription if it has one
            if (caster.entityType == Entity.EntityType.Player && item.hasBeenUsed == false)
            {
                item.hasBeenUsed = true;
            }


            switch (item.itemName)
            {
                case "Bandage":
                    roundManager.actionQueue.Enqueue("used Bandage", () => useBandage(target, caster, item)); 
                    break;
                case "Pocket Shark":
                    roundManager.actionQueue.Enqueue("used Pocket Shark", () => usePocketShark(target, caster, item, isTargettingEnemy)); 
                    break;
                case "Reagent Slug":
                    roundManager.actionQueue.Enqueue("used Reagent Slug", () => useReagentSlug(target, caster, item)); 
                    break;
                case "Elk Milk":
                    roundManager.actionQueue.Enqueue("used Elk Milk", () => useElkMilk(target, caster, item)); 
                    break;
                case "Jura's Fruit":
                    roundManager.actionQueue.Enqueue("used Jura Fruit", () => useJuraFruit(target, caster, item)); 
                    break;
                case "Slate Blade":
                    roundManager.actionQueue.Enqueue("used Slate Blade", () => useSlateBlade(target, caster, item)); 
                    break;
                default:
                    Debug.Log("Item not implemented yet.");
                    break;
            }



            //removes used item
            caster.items.Remove(item);
            
            Destroy(ButtonManager.Instance.lastButtonPressed);

        }
        else
        {
            audioManager.PlaySound(audioManager.skill_unable_sound);
            //logManager.AddLog(caster.name + " cannot use " + item.itemName + " on " + target.name + ".");
        }

        


        if (caster.currentMainActions <= 0)
        {
            roundManager.buttonManager.closeItemMenu(); //close the skill menu after the action is done
            roundManager.act_menu.SetActive(false); //close the action menu after the action is done
            //roundManager.act_menu.SetActive(false); //close the target menu after the action is done
            roundManager.EndTurn(); //end the turn if the caster has no main actions left
        }
        else
        {
            //roundManager.EnableSkillTargetingUI(true);
            roundManager.buttonManager.itemMenu.SetActive(true);
        }
    
    
    }



#region Item Effcts



    private IEnumerator useBandage(Entity target, Entity caster, Item item)
    {
        yield return new WaitForSeconds(0f);
        AudioManager.Instance.PlaySound(item.soundEffect); //play the healing sound
        target.heal(5);
        logManager.AddLog(caster.name + " used bandage on " + target.name + " for " + 5 + " HP.");
        

    }

    private IEnumerator usePocketShark(Entity target, Entity caster, Item item, bool isTargettingEnemy)
    {
        yield return new WaitForSeconds(0f);

        
        var damage = item.mainDiceRoll.Roll();
        AudioManager.Instance.PlaySound(item.soundEffect); //play the damage sound
        

        if (isTargettingEnemy) {
            foreach (var enemy in RoundManager.Instance.enemies)
            {
                enemy.takeDamage(damage);
            }
        }
        else
        {
            foreach (var ally in RoundManager.Instance.allies)
            {
                ally.takeDamage(damage);
            }
        }

        logManager.AddLog(caster.name + " used Pocket Shark on  for " + damage + " damage.");
    }
    
    private IEnumerator useReagentSlug(Entity target, Entity caster, Item item)
    {
        yield return new WaitForSeconds(0f);
        AudioManager.Instance.PlaySound(item.soundEffect); //play the healing sound

        List<Action> callbacks = new List<Action>();

        foreach (var effect in target.activeStatusEffects)
        {
            if (effect.isStackable) callbacks.Add(effect.callbackEffect);
        }

        foreach (var callback in callbacks)
        {
            callback.Invoke();
        }
        
        logManager.AddLog(caster.name + " used Reagent Slug on " + target.name );
        

    }

    private IEnumerator useElkMilk(Entity target, Entity caster, Item item)
    {
        yield return new WaitForSeconds(0f);
        AudioManager.Instance.PlaySound(item.soundEffect); 

        ActiveEffectManager.RemoveAllEffects(target.activeStatusEffects);


        logManager.AddLog(target.name + " drunk Elk Milk" );
    }

    private IEnumerator useJuraFruit(Entity target, Entity caster, Item item)
    {
        yield return new WaitForSeconds(0f);
        AudioManager.Instance.PlaySound(item.soundEffect); 

        target.heal( (int) Math.Ceiling((float)target.getMaxHP() / 100 * (int)item.extraInput));
        target.changeMP( (int) Math.Ceiling((float)target.getMaxMP() / 100 * (int)item.extraInput));

        logManager.AddLog(target.name + " ate Jura's Fruit" );

    }

    private IEnumerator useSlateBlade(Entity target, Entity caster, Item item)
    {
        yield return new WaitForSeconds(0f);
        AudioManager.Instance.PlaySound(item.soundEffect); 

        ActiveEffectManager.Instance.addSlateScar(target, caster, true);
        
    }

#endregion
}
