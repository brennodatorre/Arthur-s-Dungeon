using System;
using System.Collections;
using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;



public class MySceneManager : MonoBehaviour
{

    [HideInInspector] public static MySceneManager Instance;

    public enum SceneType { COMBAT, MAINMENU, TUTORIAL, DEATHSHOP, EVENT, TEST, NEXT }
    public SceneType currentSceneType;
    public Entity player;

    public SceneDatabase eventSceneDatabase;

    private AudioManager audioManager;
    private CursorManager cursorManager;
    public GameObject tooltipPanel;
    private GameObject lastPopUp;

    [HideInInspector] public bool isInTransition = false;
    public GameObject inputBlockerPrefab;
    private GameObject inputBlocker;
    private Canvas canvas;

    public float intentDelay = 0;
    public float popUpDuration = 3f;
    [Tooltip("Percentage of the screen height the pop up will appear from the bottom")]
    [Range(0, 1)] public float screenPercentForPopUp = 0.01f;
    public float eventRate= .3f;
    public float currentEventRate= .3f;

    









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

        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        inputBlocker = Instantiate(inputBlockerPrefab, canvas.transform);
        inputBlocker.SetActive(false);
        inputBlocker.transform.SetAsLastSibling();
    }


    public SceneType getNextScene()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);

        if (rand < currentEventRate)
        {
            currentEventRate = eventRate;   
            return SceneType.EVENT;
        }
        else
        {
            currentEventRate += 0.1f;
            return SceneType.COMBAT;
        }
    }



    public IEnumerator openSceneWithTransition(SceneType toScene, bool withDeathSound)
    {
        setInputBlocker(true);

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

        openNextScene(toScene, 1f);
    }

    /// <summary>
    /// this two helper function are used in order to call openScene with 
    /// UnityEvent type, which can only take one argument in the inspector
    /// This is being used for interactable objects atm
    /// </summary>
    public void setIntentDelay(float delay) { intentDelay = delay; }
    public void openScene()
    {
        openNextScene(SceneType.NEXT, intentDelay);
        intentDelay = 0;
    }
    public void openNextScene(SceneType toScene, float delay = 0f)
    {

        // saves the player data if coming from a combat scene
        if (currentSceneType == MySceneManager.SceneType.COMBAT) PlayerData.Instance.savePlayerData(player);
        
        
        switch (toScene)
        {
            case SceneType.COMBAT:
                StartCoroutine(openSceneWithDelay("Combat_scene", delay));
                currentSceneType = SceneType.COMBAT;
                break;

            case SceneType.TUTORIAL:
                StartCoroutine(openSceneWithDelay("Tutorial_scene", delay));
                currentSceneType = SceneType.TUTORIAL;
                break;

            case SceneType.DEATHSHOP:
                StartCoroutine(openSceneWithDelay("OutsideReader_scene", delay));
                currentSceneType = SceneType.DEATHSHOP;
                break;

            case SceneType.TEST:
                StartCoroutine(openSceneWithDelay("TESTS", delay));
                currentSceneType = SceneType.TEST;
                break;

            case SceneType.EVENT:
                string randomEvent = eventSceneDatabase.openRandom();
                StartCoroutine(openSceneWithDelay(randomEvent, delay));
                currentSceneType = SceneType.EVENT;
                break;
            case SceneType.NEXT: ///for algorithmic scene progression
                SceneType nextScene = getNextScene();
                openNextScene(nextScene, delay);
                break;

        }

    }

    private IEnumerator openSceneWithDelay(string sceneName, float delay)
    {

        yield return new WaitForSeconds(delay);


        SceneManager.LoadScene(sceneName);

        setInputBlocker(false);

    }


    private void setInputBlocker(bool state)
    {

        isInTransition = state;
        inputBlocker.SetActive(state);


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

    public void closeGame()
    {
        Application.Quit();
    }
}
