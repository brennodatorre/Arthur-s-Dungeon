using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;


public class MySceneManager : MonoBehaviour
{

    [HideInInspector] public static MySceneManager Instance;

    public enum SceneType { COMBAT, MAINMENU, TUTORIAL, DEATHSHOP, TEST }
    public SceneType sceneType;
    public Entity player;

    private AudioManager audioManager;

    public float intentDelay = 0;





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


    // this two helper function are used in order to call openScene with 
    // UnityEvent type, which can only take one argument in the inspector
    // This is being used for interactable objects atm
    public void setIntentDelay(float delay) { intentDelay = delay; }
    public void openScene(string sceneName)
    {
        openScene(sceneName, intentDelay);
        intentDelay = 0;
    }
    public void openScene(string sceneName, float delay)
    {


        //if (sceneType == SceneType.COMBAT || sceneType == SceneType.DEATHSHOP) { PlayerData.Instance.savePlayerData(player); }

        if (sceneName == "COMBAT")
        {
            StartCoroutine(openSceneWithDelay("Combat_scene", delay));
            sceneType = SceneType.COMBAT;
            
        }
        else if (sceneName == "TUTORIAL")
        {
            StartCoroutine(openSceneWithDelay("Tutorial_scene", delay));
            sceneType = SceneType.TUTORIAL;
        }
        else if (sceneName == "DEATHSHOP")
        {
            StartCoroutine(openSceneWithDelay("OutsideReader_scene", delay));
            sceneType = SceneType.DEATHSHOP;
            
        }
        else if (sceneName == "TESTS")
        {
            StartCoroutine(openSceneWithDelay("TESTS", delay));
            sceneType = SceneType.TEST;
        }


        else { Debug.Log("IRREGULAR SCENE TYPE"); }



    }

    private IEnumerator openSceneWithDelay(string sceneName, float delay)
    {

        yield return new WaitForSeconds(delay);


        SceneManager.LoadScene(sceneName);



    }



    
}
