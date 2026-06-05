using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static Properties;

public class OldRockEventManager : EventManager
{

    public GameObject dialogue;
    public GameObject leaveButton;
    public GameObject itemContainer;
    public Skill plattedSoul;

    public GameObject itemButtonPrefab;


    DungeonMemory dungeonMemory;
    TypeWriterEffect typeWriter;

    

    // Start is called before the first frame update
    void Start()
    {
        dungeonMemory = RunData.Instance.dungeonMemory;

        typeWriter = dialogue.GetComponent<TypeWriterEffect>();


    }


    public void TalkToRock()
    {
        StartCoroutine(talkToRockCoroutine()); 
        GetComponent<Button>().interactable = false; // disable the button after talking to the rock to prevent spamming the event       

    }


    public IEnumerator talkToRockCoroutine()
    {
        // first time talking to the old rock, trigger the event
        if (!dungeonMemory.HasBeenTriggered(GameEvents.TalkedToOldRock))
        {
            dungeonMemory.Trigger(GameEvents.TalkedToOldRock);

            dialogue.GetComponent<TextMeshProUGUI>().text = "...";

            dialogue.transform.parent.gameObject.SetActive(true); // activate the dialogue box

            

            typeWriter.TypeNext("...");

            typeWriter.TypeNext("...?");

            typeWriter.TypeNext("HUH?!");

            typeWriter.TypeNext("Who are you?");

            typeWriter.TypeNext("Bloody hell, you are a god forsaken ugly mug!");

            if (PlayerData.Instance.HasSkill(plattedSoul))
            {
                dungeonMemory.Trigger(GameEvents.HadPlatedSoulPriorToOldRock);
                typeWriter.TypeNext("Damn, you already have my blessing? HOW?! \n I would remeber giving my blessing to someone so ugly...");
                typeWriter.TypeNext("Well, go on then, get out of here! I don't want to talk to you anymore!");

                yield return new WaitUntil(() => typeWriter.hasFinishedTyping);
                leaveButton.SetActive(true); // show the leave button after the dialogue is done
            }
            else
            {
                yield return new WaitUntil(() => typeWriter.hasFinishedTyping);
                StartCoroutine(ProposeOriginalDeal());


                List<Item> rockyItems = PlayerData.Instance.hasItemsWithProperty(Property.ROCKY);
                if (rockyItems.Count > 0)   {StartCoroutine(ContainerPrompt(rockyItems)); }

                yield return new WaitUntil(() => typeWriter.hasFinishedTyping);
                leaveButton.SetActive(true); // show the leave button after the dialogue is done
            }

            

            yield return new WaitUntil(() => typeWriter.hasFinishedTyping);


        }
        // returning to the old rock
        else
        {
            typeWriter.TypeNext("Oh, it's you again...");
            
            List<Item> rockyItems = PlayerData.Instance.hasItemsWithProperty(Property.ROCKY);
            if (rockyItems.Count == 0)
            {
                typeWriter.TypeNext("You don't have any rocks for me... Leave!");


                yield return new WaitUntil(() => typeWriter.hasFinishedTyping);

                leaveButton.SetActive(true); // show the leave button after the dialogue is done

            }
            else
            {
                if (dungeonMemory.HasBeenTriggered(GameEvents.HadPlatedSoulPriorToOldRock))
                {
                   typeWriter.TypeNext("Well, show me the goods then!");
                     yield return new WaitUntil(() => typeWriter.hasFinishedTyping);
                    
                    StartCoroutine(ContainerPrompt(rockyItems));
                }
                else
                {
                    if (PlayerData.Instance.HasSkill(plattedSoul))
                    {
                        typeWriter.TypeNext("Hey, you already have my blessing, but you still have a rock for me?");
                        yield return new WaitUntil(() => typeWriter.hasFinishedTyping);
                        StartCoroutine(ContainerPrompt(rockyItems));
                    }
                    else
                    {
                        typeWriter.TypeNext("Oh, you actually found a rock that I like?");

                        
                        yield return new WaitUntil(() => typeWriter.hasFinishedTyping);
                        StartCoroutine(ContainerPrompt(rockyItems));
                    }
                }
            }
        }

    }

    public IEnumerator ContainerPrompt(List<Item> rockyItems )
    {
        
        {

            

            yield return new WaitUntil(() => typeWriter.hasFinishedTyping);

            itemContainer.SetActive(true);
            
            foreach (Item item in rockyItems)
            {

                GameObject buttonObj = Instantiate(itemButtonPrefab, itemContainer.transform);
                buttonObj.GetComponent<itemButtonSetter>().SetItemButton(item,"Give item?" , () =>
                {
                    StartCoroutine(giveItemCoroutine(item));

                });

            }

        
        }
    }

    public IEnumerator giveItemCoroutine(Item item)
    {
        // remove the item from the player's inventory
        PlayerData.Instance.RemoveItem(item);

        // clear the item container
        foreach (Transform child in itemContainer.transform)
        {
            Destroy(child.gameObject);
        }

        itemContainer.SetActive(false);



        typeWriter.TypeNext("Huh, you actually found a rock that I like?");

        typeWriter.TypeNext("Well, I guess I have to keep my end of the bargain then...");

        typeWriter.TypeNext("Here, take this!");

        yield return new WaitUntil(() => typeWriter.hasFinishedTyping);

        if (!PlayerData.Instance.HasSkill(plattedSoul)) {
            // give the player a reward
            PlayerData.Instance.addSkill(plattedSoul);

            StartCoroutine(MySceneManager.Instance.doPopUp( "You received the Platted Soul skill!", transform.position, Color.yellow));
        }
        else
        {
            PlayerData.Instance.changeMaxMP(5);
            StartCoroutine(MySceneManager.Instance.doPopUp( "+5 Max MP", transform.position, Color.blue));
            
        }

        yield return new WaitUntil(() => typeWriter.hasFinishedTyping);
        leaveButton.SetActive(true); // show the leave button after the dialogue is done
    
    }

    public IEnumerator ProposeOriginalDeal()
    {
        typeWriter.TypeNext("...?");
        typeWriter.TypeNext("...?");

        typeWriter.TypeNext("You look like you have been through a lot, even though I really don't care.");

        typeWriter.TypeNext("But let me at least propose a deal...");

        typeWriter.TypeNext("Bring me a nice rock, one with the right consistency and color.");

        typeWriter.TypeNext("Find me again with that rock, and I will get you something nice in return!");

        typeWriter.TypeNext("Now get out of here, I don't want to talk to you anymore!");

        yield return new WaitUntil(() => typeWriter.hasFinishedTyping);

        
    }

    public IEnumerator ProposeNewDeal()
    {

        typeWriter.TypeNext("...");
        typeWriter.TypeNext("It seems you already have my blessing! So let me give you a new deal!");
        typeWriter.TypeNext("Bring me a nice rock, one with the right consistency and color.");
        typeWriter.TypeNext("And I will get you something to make your soul even more solid!");

        
        yield return new WaitUntil(() => typeWriter.hasFinishedTyping);
    }
   
}
