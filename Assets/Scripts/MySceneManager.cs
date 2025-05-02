using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{

    public enum SceneType { COMBAT, MAINMENU, CHAT}
    public SceneType sceneType;


    public void openCombatScene()
    {

        Debug.Log("it clicked");
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene("Combat_scene");
        SceneManager.UnloadSceneAsync(currentScene);


    }


    
}
