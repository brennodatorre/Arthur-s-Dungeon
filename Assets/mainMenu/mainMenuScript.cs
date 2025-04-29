using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
    public void play(){
        SceneManager.LoadScene(1); ///
    }
    
    

    public void quitGame(){
        Application.Quit();
    }
}
