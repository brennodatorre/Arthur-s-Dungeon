using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;


public class MySceneManager : MonoBehaviour
{
    
    [HideInInspector] public static MySceneManager Instance;

    public enum SceneType { COMBAT, MAINMENU, TUTORIAL, DEATHSHOP }
    public SceneType sceneType;
    public Entity player;

    private AudioManager audioManager;




    
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

        audioManager = AudioManager.Instance;
    }



    public IEnumerator openSceneWithTransition(string toScene, bool withDeathSound){

        audioManager.ambienceOutput.Pause();
        if (withDeathSound) {audioManager.PlaySound(audioManager.death_sound);}


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

        openScene(toScene);
    }

    public void openScene(string sceneName)
    {


        if (sceneType == SceneType.COMBAT || sceneType == SceneType.DEATHSHOP) {PlayerData.Instance.savePlayerData(player);}
        
        if (sceneName == "COMBAT") {
            StartCoroutine(openCombatSceneWithDelay());
            sceneType = SceneType.COMBAT;
        }
        else if (sceneName == "TUTORIAL") {
            StartCoroutine(openTuroialSceneWithDelay());
            sceneType = SceneType.TUTORIAL;
        }
        else if (sceneName == "DEATHSHOP") {
            StartCoroutine(openDeathShopSceneWithDelay());
            sceneType = SceneType.DEATHSHOP;
        }
       else {Debug.Log("IRREGULAR SCENE TYPE");}
    


    }

    

    private IEnumerator openCombatSceneWithDelay()
    {

        yield return new WaitForSeconds(1);
        

        SceneManager.LoadScene("Combat_scene");

        

    }

    private IEnumerator openTuroialSceneWithDelay()
    {

        yield return new WaitForSeconds(1);
        

        SceneManager.LoadScene("Tutorial_scene");

        

    }

    private IEnumerator openDeathShopSceneWithDelay()
    {

        yield return new WaitForSeconds(1);
        

        SceneManager.LoadScene("OutsideReader_scene");

        

    }

    


    
}
