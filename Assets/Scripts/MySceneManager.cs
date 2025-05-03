using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;


public class MySceneManager : MonoBehaviour
{

    public enum SceneType { COMBAT, MAINMENU, TUTORIAL, DEATHSHOP}
    public SceneType sceneType;


    public void openScene(string sceneName)
    {



        
       if (sceneName == "COMBAT") {StartCoroutine(openCombatSceneWithDelay());}
       else if (sceneName == "TUTORIAL") {StartCoroutine(openTuroialSceneWithDelay());}
       else if (sceneName == "DEATHSHOP") {StartCoroutine(openDeathShopSceneWithDelay());}
       else {Debug.Log("IRREGULAR SCENE TYPE");}
    


    }

    private IEnumerator openCombatSceneWithDelay()
    {

        yield return new WaitForSeconds(1);
        

        SceneManager.LoadScene("Combat_scene");

        Scene currentScene = SceneManager.GetActiveScene();

    }

    private IEnumerator openTuroialSceneWithDelay()
    {

        yield return new WaitForSeconds(1);
        

        SceneManager.LoadScene("Tutorial_scene");

        Scene currentScene = SceneManager.GetActiveScene();

    }

    private IEnumerator openDeathShopSceneWithDelay()
    {

        yield return new WaitForSeconds(1);
        

        //SceneManager.LoadScene("Combat_scene");

        Scene currentScene = SceneManager.GetActiveScene();

    }

    


    
}
