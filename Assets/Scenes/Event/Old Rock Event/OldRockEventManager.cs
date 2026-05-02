using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OldRockEventManager : MonoBehaviour
{

    public GameObject dialogue;

    DungeonMemory dungeonMemory;

    

    // Start is called before the first frame update
    void Start()
    {
        dungeonMemory = RunData.Instance.dungeonMemory;

        


    }


    public void TalkToRock()
    {
        // first time talking to the old rock, trigger the event
        if (!dungeonMemory.HasBeenTriggered(GameEvents.TalkedToOldRock))
        {
            dungeonMemory.Trigger(GameEvents.TalkedToOldRock);

            dialogue.GetComponent<TextMeshProUGUI>().text = "...";

            dialogue.transform.parent.gameObject.SetActive(true); // activate the dialogue box

            TypeWriterEffect typeWriter = dialogue.GetComponent<TypeWriterEffect>();

            typeWriter.TypeNext("...");

            typeWriter.TypeNext("...?");

            typeWriter.TypeNext("HUH?!");

            typeWriter.TypeNext("Who are you?");

            typeWriter.TypeNext("Bloody hell, you are a god forsaken ugly mug!");

            typeWriter.TypeNext("...?");
            typeWriter.TypeNext("...?");

            typeWriter.TypeNext("You look like you have been through a lot, even though I don't care.");

            typeWriter.TypeNext("But let me at least propose a deal...");

            typeWriter.TypeNext("Bring me a nice rock, one with the right consitencey and color.");

            typeWriter.TypeNext("Find me again with that rock, and I will get you something nice in return!");

            typeWriter.TypeNext("Now get out of here, I don't want to talk to you anymore!");

            


        }
    }

   
}
