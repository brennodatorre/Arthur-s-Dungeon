using System.Collections;
using System.Collections.Generic;
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
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }
    }


    public void useItem( Entity target, Entity caster, Item item)
    {
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

            switch (item.itemName)
            {
                case "Bandage":
                    roundManager.actionQueue.Enqueue("use Bandage", () => useBandage(target, caster, item)); //add the action to the queue
                    break;

                default:
                    Debug.Log("Skill not implemented yet.");
                    break;
            }

        }
        else
        {
            audioManager.PlaySound(audioManager.skill_unable_sound);
            logManager.AddLog(caster.name + " cannot use " + item.itemName + " on " + target.name + ".");
        }

        //removes used item
        caster.items.Remove(item);
        caster.itemsInstance.Remove(item);
        Destroy(ButtonManager.Instance.lastButtonPressed);


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


    private IEnumerator useBandage(Entity target, Entity caster, Item item)
    {
        yield return null;
        target.heal(5);
        logManager.AddLog(caster.name + " used bandage on " + target.name + " for " + 5 + " HP.");
    }
    
}
