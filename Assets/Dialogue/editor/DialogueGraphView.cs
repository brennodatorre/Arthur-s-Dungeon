using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Codice.Client.Common.FsNodeReaders;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

public class DialogueGraphView : GraphView
{

    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);

    public Blackboard Blackboard;
    public List<ExposedProperty> exposedProperties= new List<ExposedProperty>();

    private NodeSearchWindow _searchWindow;

    private SerializableDictionary<string, NodeErrorData> ungroupedNodes;

    private EditorWindow _editorWindow;


    public DialogueGraphView(EditorWindow editorWindow){
        
        _editorWindow = editorWindow;

        ungroupedNodes = new SerializableDictionary<string,NodeErrorData>();

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

       AddElement( GenerateEntryNode());
       AddSearchWindow(editorWindow);

       this.AddManipulator(CreateGroupContextMenu());
    }

    private IManipulator CreateGroupContextMenu()
    {
        

        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Add Group", evt => AddElement(CreateGroup("Dialogue Group", GetLocalMousePosition(evt.eventInfo.localMousePosition))))
        );
        return contextualMenuManipulator;
    }

    public Group CreateGroup(string tittle, Vector2 localmousepos)
    {
        Group group = new Group(){title = tittle};
        group.SetPosition(new Rect(localmousepos, Vector2.zero));

         return group;
    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Init(editorWindow, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach( (port) => {
            
            if(startPort!=port && startPort.node!= port.node){
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }

    private Port GeneratePort (DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    public DialogueNode GenerateEntryNode(){

        var node = new DialogueNode{
            
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            dialogText = "entry",
            entry = true

        };

        var generatedPort = (GeneratePort(node, Direction.Output));
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();


        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public void CreateNode(string nodeName, Vector2 mousePosition){
        AddElement(CreateDialogueNode(nodeName, mousePosition));

    }

    public DialogueNode CreateDialogueNode( string nodename, Vector2 mousePosition){
        var dialogueNode = new DialogueNode{
            title = nodename,
            dialogText = nodename,
            GUID = Guid.NewGuid().ToString(),

        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node_"));


        var button = new Button(() => {AddChoicePort(dialogueNode);});
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);


        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt=>
        {
            


            dialogueNode.dialogText = evt.newValue; //might need to change this to split name from dialogue
            dialogueNode.title = evt.newValue;

            this.RemoveUngroupedNode(dialogueNode);
            dialogueNode.name = evt.newValue;
            this.AddUngroupedNode(dialogueNode);



        });
        textField.SetValueWithoutNotify(dialogueNode.title);
        dialogueNode.mainContainer.Add(textField);




        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect( mousePosition, defaultNodeSize ));

        AddUngroupedNode(dialogueNode);

        return dialogueNode;

    }
#region Error Color Nodes
    public void AddUngroupedNode(DialogueNode dialogueNode) //ta meio quebrado.
    {
        string nodeName = dialogueNode.name;

        if (!ungroupedNodes.ContainsKey(nodeName))
        {
            NodeErrorData nodeErrorData = new NodeErrorData();
            nodeErrorData.nodes.Add(dialogueNode); 
            ungroupedNodes.Add(nodeName, nodeErrorData);
            return;  
        }

       
            //deals with nodes with the same name

            
            ungroupedNodes[nodeName].nodes.Add(dialogueNode);
            Color errorColor = ungroupedNodes[nodeName].ErrorColor.color;
            dialogueNode.setErrorStyle(errorColor);

            if(ungroupedNodes[nodeName].nodes.Count == 2)
            {
                ungroupedNodes[nodeName].nodes[0].setErrorStyle(errorColor);
            }
       

    }

    public void RemoveUngroupedNode(DialogueNode node){

        string nodeName = node.name;

        ungroupedNodes[nodeName].nodes.Remove(node);

        node.ResetStyle();

        if(ungroupedNodes[nodeName].nodes.Count == 1){
            ungroupedNodes[nodeName].nodes[0].ResetStyle();
            return ;

        }

        if (ungroupedNodes[nodeName].nodes.Count == 1){

            ungroupedNodes.Remove(nodeName);
        }
    }


#endregion
    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortname = "") {

        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);


        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = $"Choice {outputPortCount}";

        var choicePortName = string.IsNullOrEmpty(overriddenPortname) ? $"Choice {outputPortCount+1}" : overriddenPortname;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);
        textField.style.minWidth = 60; //this 2 lines deals with sizing inside the node, just let the struct be more stable
        textField.style.maxWidth = 100;
        var deletedButton = new Button(()=> RemovePort(dialogueNode, generatedPort))
        {
            text = "X"

        };   
        generatedPort.contentContainer.Add(deletedButton);




        generatedPort.portName = choicePortName;

        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x=>x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);


        if (targetEdge.Any()) {
        var edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();

    }

    internal void AddPropertyToBlackBoard(ExposedProperty exposedProperty)
    {
        var localPropertyName = exposedProperty.PropertyName;
        var localPropertyValue = exposedProperty.PropertyValue;
        while (exposedProperties.Any(x => x.PropertyName == localPropertyName ))
        {
            localPropertyName = $"{localPropertyName}(1)";
        }

        var property = new ExposedProperty();
        property.PropertyName = localPropertyName;
        property.PropertyValue = localPropertyValue;
        exposedProperties.Add(property);

        var container = new VisualElement();

        var blackboardField = new BlackboardField{text = property.PropertyName, typeText = "string property"};
        container.Add(blackboardField);

        var propertyValueTextField = new TextField ("Value"){
            value= localPropertyValue
        };

        propertyValueTextField.RegisterValueChangedCallback(evt => {
            var changingPropertyIndex = exposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
            exposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
        });

        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);


        Blackboard.Add(container);
    }

    public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent, mousePosition - _editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }




}
