using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class DialogueGraphWindow : EditorWindow
{
    private DialogueGraphView _graphView;
    private DialogueGraph currentGraph;
    private const string LastGraphKey = "DialogueGraphWindow_LastGraph";

    [MenuItem("Window/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraphWindow>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();


        // Try to restore last opened graph
        string path = EditorPrefs.GetString(LastGraphKey, "");
        if (!string.IsNullOrEmpty(path))
        {
            var graph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(path);
            if (graph != null)
            {
                currentGraph = graph;
                _graphView.LoadGraph(currentGraph);
            }
        }
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar(); 

        // Create new DialogueNode
        var nodeButton = new Button(() => { _graphView.CreateNode("Dialogue"); })
        {
            text = "Create Dialogue Node"
        };
        toolbar.Add(nodeButton);

        // New Graph
        var newButton = new Button(() =>
        {
            string path = EditorUtility.SaveFilePanelInProject("New Dialogue Graph", "NewDialogueGraph", "asset", "Choose a save location");
            if (!string.IsNullOrEmpty(path))
            {
                currentGraph = ScriptableObject.CreateInstance<DialogueGraph>();
                AssetDatabase.CreateAsset(currentGraph, path);
                AssetDatabase.SaveAssets();
                EditorPrefs.SetString(LastGraphKey, path); // save path
                _graphView.LoadGraph(currentGraph);
            }
        })
        { text = "New Graph" };
        toolbar.Add(newButton);

        // Load Graph
        var loadButton = new Button(() =>
        {
            string path = EditorUtility.OpenFilePanel("Load Dialogue Graph", "Assets", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path); // make it Unity-friendly
                currentGraph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(path);
                if (currentGraph != null)
                {
                    EditorPrefs.SetString(LastGraphKey, path); // save path
                    _graphView.LoadGraph(currentGraph);
                }
            }
        })
        { text = "Load Graph" };
        toolbar.Add(loadButton);

        // Save Graph
        var saveButton = new Button(() =>
        {
            if (currentGraph != null)
            {
                _graphView.SaveGraph();
            }
            else
            {
                Debug.LogWarning("No DialogueGraph loaded. Use New or Load first.");
            }
        })
        { text = "Save Graph" };
        toolbar.Add(saveButton);

        rootVisualElement.Add(toolbar);
    }
}
