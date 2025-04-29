using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeErrorData
{
    public ErrorColorGen ErrorColor {get; set;}

    public List<DialogueNode> nodes{get; set;}

    public NodeErrorData() {
        ErrorColor = new ErrorColorGen();
        nodes = new List<DialogueNode>();
    }

    
}