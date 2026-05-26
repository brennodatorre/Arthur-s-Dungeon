using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Or_Manager : MonoBehaviour
{

    public GameObject shop;
    public GameObject shop_background;
    public GameObject dialogue;

    TypeWriterEffect typeWriter;
    DungeonMemory dungeonMemory;
    
  
    

    // Start is called before the first frame update
    void Start()
    {
        typeWriter = dialogue.GetComponentInChildren<TypeWriterEffect>();
        dungeonMemory = RunData.Instance.dungeonMemory;


        StartCoroutine(InteractWithOR());
    }


    public IEnumerator InteractWithOR()
    {
        if (!dungeonMemory.HasBeenTriggered(GameEvents.hasVisitedOR))
        {
            dungeonMemory.Trigger(GameEvents.hasVisitedOR);

            

            dialogue.transform.parent.gameObject.SetActive(true); // activate the dialogue box

            

            typeWriter.TypeNext("...");
            typeWriter.TypeNext("It seems that you have found your way into a dangerous situation...");
            typeWriter.TypeNext("Given your circumstances, I guess you were prepared for this...");

            typeWriter.TypeNext("No need to worry, you can still go back.");
            typeWriter.TypeNext("Agamenon is not here at the moment...");
            typeWriter.TypeNext("Its not the time for someone like you to be embraced by the unknown... At least, not yet...");

            typeWriter.TypeNext("You can think of me as an OUTSIDER, someone that is here only to observe without directly interfering.");
            typeWriter.TypeNext("Though, there are some things the deal I made still allows me to do...");

            typeWriter.TypeNext("I have a few things that might be of use to you and your story.");
            typeWriter.TypeNext("I will just need pices of your FABLE as means of payment.");

            typeWriter.TypeNext("Have a look, and remenber that you are allowed to leave as you please...");


            
       }
       else
        {
            dialogue.transform.parent.gameObject.SetActive(true); // activate the dialogue box

            typeWriter.TypeNext("You are back...");

            typeWriter.TypeNext("I hope you can find something useful this time around...");
        }


        yield return new WaitUntil(() => typeWriter.hasFinishedTyping);
        dialogue.SetActive(false); 
        openShop(); // open the shop after the dialogue is done

    }

    

    public void openShop(){
        

        shop.SetActive(true);
        shop_background.SetActive(true);


    }

    public void closeShop(){
        shop.SetActive(false);
        shop_background.SetActive(false);

        
    }

   

  



    public void leaveOutsideReaderDomain(){
        PlayerData.Instance.saveJsonData();
        Debug.Log(Application.persistentDataPath);
        StartCoroutine(MySceneManager.Instance.openSceneWithTransition(MySceneManager.SceneType.NEXT));
    }
}
