using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{

    


    [SerializeField] public GameObject customCursor;
    [SerializeField] private Canvas canvas; 
    [SerializeField]private Sprite base_cursor; // The default cursor sprite
    [SerializeField]private Sprite onClick_cursor; // The clicked cursor sprite
    [SerializeField]private Sprite blade_cursor; // The hovered cursor sprite

    public RoundManager roundManager;
    public MySceneManager sceneManager;
    

    public int base_cursorSize = 1; // Size of the cursor in pixels
    public int onClick_cursorSize = 1; // Size of the cursor in pixels
    public int blade_cursorSize = 1; // Size of the cursor in pixels

    void Start() {
        
        Cursor.visible = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        

    }


    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        // Convert the mouse position to canvas space
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), mousePos, canvas.worldCamera, out localPoint);



        // Update the custom cursor's anchored position to match the mouse position
        customCursor.GetComponent<RectTransform>().anchoredPosition = localPoint;
        // Adjust the position to center the cursor
        customCursor.GetComponent<RectTransform>().anchoredPosition += new Vector2(32, -44); 

        
        if ( sceneManager.sceneType == MySceneManager.SceneType.COMBAT && roundManager.currentPhase == RoundManager.TurnPhase.targetingATK) // if the targeting phase is active
        {
            customCursor.GetComponent<Image>().sprite = blade_cursor; // Change to hovered cursor sprite
            // Adjust the position to center the cursor
            customCursor.GetComponent<RectTransform>().anchoredPosition += new Vector2(-32, 44);
            updateCursorScale(blade_cursorSize); // change the size of the cursor
        }
        // else if (roundManager.currentPhase == RoundManager.TurnPhase.targetingSKILL) // if the targeting phase is active
        // {
            
        // }
        else if (Input.GetMouseButton(0)) // left click
        {
            customCursor.GetComponent<Image>().sprite = onClick_cursor; // Change to clicked cursor sprite
            updateCursorScale(onClick_cursorSize); // change the size of the cursor
        }
        else 
        {
            customCursor.GetComponent<Image>().sprite = base_cursor; // Change back to default cursor sprite
            updateCursorScale(base_cursorSize); // change the size of the cursor
        }


    }

    private void updateCursorScale(float scale)
    {
        customCursor.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
    }
}
