using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class MySceneManager : MonoBehaviour
{

    [HideInInspector] public static MySceneManager Instance;

    public enum SceneType { COMBAT, MAINMENU, TUTORIAL, DEATHSHOP, EVENT, TEST }
    public SceneType currentSceneType;
    public Entity player;

    public SceneDatabase eventSceneDatabase;

    private AudioManager audioManager;
    private CursorManager cursorManager;
    public GameObject tooltipPanel;
    private GameObject lastPopUp;

    public float intentDelay = 0;
    public float popUpDuration = 3f;
    [Range(0, 1)] public float screenPercentForPopUp = 0.01f;







    /// //////////////////////////////////////////////////////////////////////////////////////////////


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

    void Start()
    {
        audioManager = AudioManager.Instance;
        cursorManager = CursorManager.Instance;


    }



    public IEnumerator openSceneWithTransition(string toScene, bool withDeathSound)
    {

        audioManager.ambienceOutput.Pause();
        if (withDeathSound) { audioManager.PlaySound(audioManager.death_sound); }


        GameObject blackout = GameObject.Find("BlackoutOverlay");
        SpriteRenderer spriteRenderer = blackout.GetComponent<SpriteRenderer>();

        float duration = 5f;
        float currentTimer = 0f;

        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);


        while (currentTimer < duration)
        {
            currentTimer += Time.deltaTime;

            float t = currentTimer / duration;

            spriteRenderer.color = Color.Lerp(startColor, endColor, t);

            yield return null; // wait for the next frame

        }


        spriteRenderer.color = endColor;

        yield return new WaitForSeconds(3f);

        openScene(toScene, 1f);
    }

    /// <summary>
    /// this two helper function are used in order to call openScene with 
    /// UnityEvent type, which can only take one argument in the inspector
    /// This is being used for interactable objects atm
    /// </summary>
    public void setIntentDelay(float delay) { intentDelay = delay; }
    public void openScene(string sceneName)
    {
        openScene(sceneName, intentDelay);
        intentDelay = 0;
    }
    public void openScene(string sceneName, float delay)
    {

        // saves the player data if coming from a combat scene
        if (currentSceneType == MySceneManager.SceneType.COMBAT) PlayerData.Instance.savePlayerData(player);
        


        if (sceneName == "COMBAT")
        {
            StartCoroutine(openSceneWithDelay("Combat_scene", delay));
            currentSceneType = SceneType.COMBAT;
            
        }
        else if (sceneName == "TUTORIAL")
        {
            StartCoroutine(openSceneWithDelay("Tutorial_scene", delay));
            currentSceneType = SceneType.TUTORIAL;
        }
        else if (sceneName == "DEATHSHOP")
        {
            StartCoroutine(openSceneWithDelay("OutsideReader_scene", delay));
            currentSceneType = SceneType.DEATHSHOP;
            
        }
        else if (sceneName == "TESTS")
        {
            StartCoroutine(openSceneWithDelay("TESTS", delay));
            currentSceneType = SceneType.TEST;
        }
        else if (sceneName == "EVENT")
        {
            string randomEvent = eventSceneDatabase.openRandom();
            StartCoroutine(openSceneWithDelay(randomEvent, delay));
            currentSceneType = SceneType.EVENT;
        }



        else { StartCoroutine(openSceneWithDelay(sceneName, delay));}



    }

    private IEnumerator openSceneWithDelay(string sceneName, float delay)
    {

        yield return new WaitForSeconds(delay);


        SceneManager.LoadScene(sceneName);



    }


    /// <summary>
    /// Does a pop up with the inputed message, which stays for popUpDuration seconds
    /// </summary>
    public IEnumerator doPopUp(String message)
    {
        if (lastPopUp != null)
        {
            Destroy(lastPopUp);
        }

        //creates a new tooltip panel
        GameObject popUpPanel = Instantiate(this.tooltipPanel, tooltipPanel.transform.parent);
        popUpPanel.transform.SetAsLastSibling(); //makes sure it's on top of other UI elements

        lastPopUp = popUpPanel; //saves referejce

        popUpPanel.GetComponentInChildren<TextMeshProUGUI>().text = message;

        
        popUpPanel.transform.position += new Vector3(0, Screen.height * screenPercentForPopUp, 0); //moves it a bit up so it's not right on the cursor

        yield return new WaitForSeconds(popUpDuration);

        Destroy(popUpPanel);
    }

    
}
