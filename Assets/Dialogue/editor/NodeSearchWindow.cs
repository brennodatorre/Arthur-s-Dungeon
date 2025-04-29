using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{

    private DialogueGraphView _graphView;
    private EditorWindow _window;

    public void Init(EditorWindow window, DialogueGraphView graphView){
        _graphView = graphView;
        _window = window;
    }


    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        

        var tree = new List<SearchTreeEntry>{
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
            new SearchTreeEntry(new GUIContent("Dialogue Node"))
            {
                userData = new DialogueNode(), level = 2
            }
            
            
        };
        return tree;

    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

        switch (SearchTreeEntry.userData)
        {
            case DialogueNode dialogueNode:
                _graphView.CreateNode("Dialogue Node", localMousePosition);
                return true;

           

            default:
                return false;
        }


    }
}
