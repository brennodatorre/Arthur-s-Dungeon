using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    

    
    void Update() {
        
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Tab or I pressed");

            InfoDisplayManager.Instance.ToggleInfoMenu();
        }
    }

    
}
