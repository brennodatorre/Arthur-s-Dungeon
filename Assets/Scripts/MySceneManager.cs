using System;
using System.Collections;
using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;



public class MySceneManager : MonoBehaviour
{

    [HideInInspector] public static MySceneManager Instance;

  
    private int currentSceneIndex;
    private bool sceneChanged;

    public enum SceneType { COMBAT, MAINMENU, TUTORIAL, DEATHSHOP, EVENT, TEST, NEXT, MAP }
    public SceneType currentSceneType;
    public Entity player;

    public SceneDatabase eventSceneDatabase;

    private AudioManager audioManager;
    private CursorManager cursorManager;

    private GameObject blackout;
    public GameObject tooltipPanelPrefab;

    public GameObject popUpPrefab;


    private GameObject lastPopUp;

    [HideInInspector] public bool isInTransition = false;
    [HideInInspector] public bool halfWayInTransition  = false;
    public GameObject inputBlockerPrefab;
    [HideInInspector] public GameObject inputBlocker;
    [HideInInspector] public Canvas canvas;

    public float intentDelay = 0;
    public float popUpDuration = 3f;
    [Tooltip("Percentage of the screen height the pop up will appear from the bottom")]
    [Range(0, 1)] public float screenPercentForPopUp = 0.01f;
    public float eventRate= .3f;
    public float currentEventRate= .3f;

    public float fadeDuration =2f;

    









    /// //////////////////////////////////////////////////////////////////////////////////////////////


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

    void Start()
    {
        setupScene();        

        currentSceneIndex =SceneManager.GetActiveScene().buildIndex;

       
    }

    void Update()
    {
         

        // reload references if scene has changed
        if (currentSceneIndex != SceneManager.GetActiveScene().buildIndex || sceneChanged) {  
            setupScene(); 
        }



        currentSceneIndex =SceneManager.GetActiveScene().buildIndex;
         
    } 


    private void setupScene()
    {
        sceneChanged = false;
        audioManager = AudioManager.Instance;
        cursorManager = CursorManager.Instance;

        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

        if (!isInTransition) audioManager.setVolume();

        inputBlocker = Instantiate(inputBlockerPrefab, canvas.transform);
        inputBlocker.SetActive(false);
        inputBlocker.transform.SetAsLastSibling();


        if (currentSceneType == SceneType.COMBAT)
        {
            player = RoundManager.Instance.player;
        }

        if (blackout == null)
        {
            blackout = transform.GetChild(0).transform.Find("BlackoutOverlay").gameObject;
        }

        // reset blackout alpha
        if (blackout.GetComponent<SpriteRenderer>().color.a != 0)
        {
            StartCoroutine(doFadeOverlay(blackout.GetComponent<SpriteRenderer>(), new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), fadeDuration));
        }
    }


    public SceneType getNextScene()
    {
        if (currentSceneType != SceneType.MAP) {return SceneType.MAP;}

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



    public IEnumerator openSceneWithTransition(SceneType toScene )
    {
        
        

        if (blackout == null)
            {
                blackout = transform.GetChild(0).transform.Find("BlackoutOverlay").gameObject;
            }

        yield return StartCoroutine(doFadeOverlay(blackout.GetComponent<SpriteRenderer>(), new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), fadeDuration));

        yield return new WaitForSeconds(3f);

        openNextScene(toScene, intentDelay);
    }

    /// <summary>
    /// this two helper function are used in order to call openScene with 
    /// UnityEvent type, which can only take one argument in the inspector
    /// This is being used for interactable objects atm
    /// </summary>
    public static void setIntentDelay(float delay) { MySceneManager.Instance.intentDelay = delay; }
    public static void openScene(){MySceneManager.Instance.openSceneT();}
    private void openSceneT()
    {
        StartCoroutine(MySceneManager.Instance.openSceneWithTransition(SceneType.NEXT));
        MySceneManager.Instance.intentDelay = 0;
    }
    public static void openMainMenuScene() { MySceneManager.Instance.StartCoroutine(MySceneManager.Instance.openSceneWithTransition(SceneType.MAINMENU)); }
    public void openNextScene(SceneType toScene, float delay = 1f)
    {

        // saves the player data if coming from a combat scene
        if (currentSceneType == MySceneManager.SceneType.COMBAT) PlayerData.Instance.savePlayerData(player);
        
        
        switch (toScene)
        {
            case SceneType.COMBAT:
                currentSceneType = SceneType.COMBAT;
                StartCoroutine(openSceneWithDelay("Combat_scene", delay));
                
                break;

            case SceneType.TUTORIAL:
                currentSceneType = SceneType.TUTORIAL;
                StartCoroutine(openSceneWithDelay("Tutorial_scene", delay));
                
                break;

            case SceneType.DEATHSHOP:
                currentSceneType = SceneType.DEATHSHOP;
                StartCoroutine(openSceneWithDelay("OutsideReader_scene", delay));
                
                break;

            case SceneType.TEST:
                currentSceneType = SceneType.TEST;
                StartCoroutine(openSceneWithDelay("TESTS", delay));
                
                break;

            case SceneType.EVENT:
                string randomEvent = eventSceneDatabase.getRandom();
                Debug.Log("Opening event scene: " + randomEvent);
                currentSceneType = SceneType.EVENT;
                StartCoroutine(openSceneWithDelay(randomEvent, delay));
                
                break;
            case SceneType.NEXT: ///for algorithmic scene progression
                SceneType nextScene = getNextScene();
                openNextScene(nextScene, delay);
                break;

            case SceneType.MAP:
                StartCoroutine(openSceneWithDelay("Map Scene", delay));
                currentSceneType = SceneType.MAP;
                break;
            case SceneType.MAINMENU:
                StartCoroutine(openSceneWithDelay("MainMenu_scene", delay));
                currentSceneType = SceneType.MAINMENU;
                break;


        }

    }

    private IEnumerator openSceneWithDelay(string sceneName, float delay)
    {
        
        yield return new WaitForSeconds(delay);

        sceneChanged= true;
        SceneManager.LoadScene(sceneName);

        

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
        GameObject popUpPanel = Instantiate(tooltipPanelPrefab, canvas.transform);
        popUpPanel.transform.position = CursorManager.Instance.customCursor.transform.position;
        popUpPanel.transform.SetAsLastSibling(); //makes sure it's on top of other UI elements

        lastPopUp = popUpPanel; //saves referejce

        popUpPanel.GetComponentInChildren<TextMeshProUGUI>().text = message;

        
        popUpPanel.transform.position += new Vector3(0, Screen.height * screenPercentForPopUp, 0); //moves it a bit up so it's not right on the cursor

        yield return new WaitForSeconds(popUpDuration);

        Destroy(popUpPanel);
    }

    public IEnumerator doFadeOverlay( SpriteRenderer spriteRenderer,  Color initialColor, Color finalColor, float duration = 2f)
    {
        setInputBlocker(true);
        
        float currentTimer = 0f;

        Color startColor = initialColor;
        Color endColor = finalColor;

        while (currentTimer < duration)
        {
            currentTimer += Time.deltaTime;

            if (currentTimer > duration /2 && !halfWayInTransition ) {halfWayInTransition = true;}

            float t = currentTimer / duration;

            spriteRenderer.color = Color.Lerp(startColor, endColor, t);

            yield return null; // wait for the next frame

        }


        spriteRenderer.color = endColor;

        setInputBlocker(false);

    }

    public IEnumerator doPopUp(String text, Vector3 position, Color color)
    {
        yield return new WaitForSeconds(0.2f); // delay before showing the popup

        GameObject popup = Instantiate(popUpPrefab, canvas.transform);
        popup.GetComponentInChildren<TextMeshProUGUI>().text = text;
        popup.GetComponentInChildren<TextMeshProUGUI>().color = color;
        popup.transform.position = position;
        
    }


    public void closeGame()
    {
        Application.Quit();
    }

    
 

}
