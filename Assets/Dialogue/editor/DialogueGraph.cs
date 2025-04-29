using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using System.Linq;

public class DialogueGraph : EditorWindow {

    private DialogueGraphView _graphView;
    private string _fileName;

    [MenuItem("Graph/Dialogue Graph")]
    public static void openDialogueGraphWindow() 
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }


    private void OnEnable() {
        
        ConstructGraphView();
        GenToolBar();
        GenerateMiniMap();
        GenBlackBoard();

    }

    private void GenBlackBoard()
    {
        var blackBoard  = new Blackboard(_graphView);
        blackBoard.Add(new BlackboardSection{title = "Exposed Properties"});
        blackBoard.addItemRequested = _blackboard => {_graphView.AddPropertyToBlackBoard( new ExposedProperty());};
        blackBoard.editTextRequested = (blacboard1, element, newValue) => {
            var oldPropertyName = ((BlackboardField) element).text;
            if(_graphView.exposedProperties.Any(x => x.PropertyName == newValue))
            {
                EditorUtility.DisplayDialog("Error", "This property name already exists", "ok!");
                return;
            }

            var propertyIndex = _graphView.exposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
            _graphView.exposedProperties[propertyIndex].PropertyName = newValue;
            ((BlackboardField) element).text = newValue;
        };
        blackBoard.SetPosition(new Rect(10,30,200,300));

        _graphView.Add(blackBoard);
        _graphView.Blackboard = blackBoard;
    }

    

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap{ anchored = true};
        //this will give 10px offset from left side
        var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(position.width - 210, 30)); //currently, if window changes size, this does not follow up fiz//////////////////////////////////////////////////////////////////////////////////////////////
        miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        _graphView.Add(miniMap);
    }

    private void ConstructGraphView(){

        _graphView = new DialogueGraphView(this) 
        {
            name = "Dialogue Graph",
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);


    }

    private void GenToolBar(){

        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(()=>RequestDataOperation(true)){text = "Save Datda"});
        toolbar.Add(new Button(()=>RequestDataOperation(false)){text = "Load Datda"});


        rootVisualElement.Add(toolbar);


    }

    
    private void RequestDataOperation(bool save){
        if (string.IsNullOrEmpty(_fileName)){
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name", "OK");
            return;
        }

        var saveUtility = GraphSaveUtil.GetInstance(_graphView);
        if (save){
            saveUtility.SaveGraph(_fileName);
        }
        else {
            saveUtility.LoadGraph(_fileName);
        }

    }
   




    private void OnDisable() {

        rootVisualElement.Remove(_graphView);
        
    }


    


}

