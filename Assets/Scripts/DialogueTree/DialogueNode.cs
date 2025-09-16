using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode : ScriptableObject
{
    public string GUID; // unique identifier
    public Sprite image;
    public bool hasHappened = false; // tracks if this node has been used/display to player
    [TextArea(2, 5)]
    public string dialogue;

    public List<string> options = new List<string>();  // stores GUIDs of child nodes
    public string previousDialogue; // stores GUID of parent

    [SerializeField]
    public List<DialogueCondition> conditions = new List<DialogueCondition>(); // All must be met to display node

    [HideInInspector]public Rect nodePosition = new Rect(100, 200, 250, 150); // editor position
}
