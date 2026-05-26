using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FountainEventManager : MonoBehaviour
{

    public int healAmountPercentage = 10;
    public int MaxHPPercentageLoss = 5;

    public int currentSuccessRate = 100;
    public int successRateDecrease = 15;

    public int drunkAmount = 0;

    [Range(0,5)]public float picthRange = 1f;
    public AudioClip gulpSound;
    public AudioClip fountainAmbience;

    private AudioSource fountainAmbienceASource;


    public GameObject dialogue;
    TypeWriterEffect typeWriter;
    Button fountainBtn;



    // Start is called before the first frame update
    void Start()
    {
        if (fountainAmbience != null ) { fountainAmbienceASource = AudioManager.Instance.CreateAndPlaySound(fountainAmbience, true);}

        typeWriter = dialogue.GetComponentInChildren<TypeWriterEffect>();
        fountainBtn = this.GetComponentInChildren<Button>();
        fountainBtn.enabled = false;

                   
        StartCoroutine(FountainIntro());
        
    }



    private IEnumerator FountainIntro()
    {
        dialogue.transform.parent.gameObject.SetActive(true); // activate the dialogue box

        typeWriter.TypeNext("You find yourself in front of a fountain full of a dark blood. \n You can smell and see a faint red miasma coming from it");

        typeWriter.TypeNext("Drink from the fountain?");

        yield return new WaitUntil(() => typeWriter.hasFinishedTyping);

        fountainBtn.enabled = true;
    }

    

    public void InteractWithFountain()
    {

        bool success = Random.Range(0, 100) < currentSuccessRate;


        // After 6 interactions, the fountain becomes unusable and gives a permanent DEF buff
        if (drunkAmount >= 6) { 
            
            GetComponent<Button>().interactable = false;

            GetComponent<Image>().color = Color.gray;

            PlayerData.Instance.changeDEF(1);

            StartCoroutine(MySceneManager.Instance.doPopUp("+" + 1 + " DEF", this.transform.position, Color.white));

            fountainAmbienceASource.Stop();
            Destroy(fountainAmbienceASource);

        }
        else if (success)
        {
                
            int healAmount = Mathf.CeilToInt(PlayerData.Instance.getMaxHP() * (healAmountPercentage / 100f));

            PlayerData.Instance.healPlayer(healAmount);

            StartCoroutine(MySceneManager.Instance.doPopUp(healAmount.ToString(), this.transform.position, Color.green));
        }
        else
        {
            int maxHPLost = Mathf.CeilToInt(PlayerData.Instance.getMaxHP() * (MaxHPPercentageLoss / 100f));

            PlayerData.Instance.changeMaxHP(-maxHPLost);

            StartCoroutine(MySceneManager.Instance.doPopUp("-" + maxHPLost.ToString() + " MaxHP", this.transform.position, Color.red));
        }

        currentSuccessRate -= successRateDecrease;
        drunkAmount++;

        float rand = Random.Range(-.5f,.5f);
        AudioManager.Instance.PlaySoundWithPich(gulpSound, picthRange + rand);

        




    }
}
