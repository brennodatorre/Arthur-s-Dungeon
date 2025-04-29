using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : Node
{
    public string GUID;

    public string dialogText;

    public bool entry = false;


    public void setErrorStyle(Color color){

        mainContainer.style.backgroundColor = color; 

    }


    public void ResetStyle(){
        //mainContainer.styleSheets.Add(Resources.Load<StyleSheet>("Node_"));
        mainContainer.style.backgroundColor = new Color(10, 10, 10, 250); 
    }
}
