using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using log4net.Appender;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtil 
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList(); //the <Edge> might be using wrong lib
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList(); // cast is using a lib linq, might be wrong

    public static GraphSaveUtil GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtil
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string filename){

        if (!Edges.Any()) return; //if there are no adges, then return

        var DialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        var connectedPorts = Edges.Where (x=> x.input.node != null).ToArray();
        for (var i = 0; i < connectedPorts.Length; i++){
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            DialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGUID = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            });
        }

        foreach (var dialogueNode in Nodes.Where(node=>!node.entry)){

            DialogueContainer.DialogueNodeData.Add(new DialogueNodeData{

                GUID = dialogueNode.GUID,
                DialogueText = dialogueNode.dialogText,
                Position = dialogueNode.GetPosition().position
            });
        }

        //Auto creates resources folder if it does not exist
        if (!AssetDatabase.IsValidFolder("Assets/Dialogue/Resources"))
        {
            AssetDatabase.CreateFolder("Dialogue", "Resources");
        }


        AssetDatabase.CreateAsset(DialogueContainer, $"Assets/Dialogue/Resources/{filename}.asset");
        AssetDatabase.SaveAssets(); 



    }

    public void LoadGraph(string filename){

        _containerCache = Resources.Load<DialogueContainer>(filename);
        if (_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file did not exist", "OK");
            return;
        }


        ClearGraph();
        CreateNodes();
        ConnectNodes();


    }

    private void ConnectNodes()
    {
        for(var i = 0; i < Nodes.Count; i++){

            var connections = _containerCache.NodeLinks.Where(x=> x.BaseNodeGUID == Nodes[i].GUID).ToList();
            for (var j = 0; j < connections.Count; j++ ){
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x=> x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect (
                    _containerCache.DialogueNodeData.First(x=> x.GUID == targetNodeGuid).Position, _targetGraphView.defaultNodeSize
                ));
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge =  new Edge
        {
            output = output,
            input = input
        };
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        
        _targetGraphView.Add(tempEdge);
    }

    private void CreateNodes()
    {
        foreach (var nodeData in _containerCache.DialogueNodeData)
        {
            //passing position later on, so we can use a temp vec.zero while loading node
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText, Vector2.zero);
            tempNode.GUID = nodeData.GUID;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x=>x.BaseNodeGUID == nodeData.GUID).ToList();
            nodePorts.ForEach(x=>_targetGraphView.AddChoicePort(tempNode, x.PortName));   
        }
    }

    private void ClearGraph()
    {
        //sets entry points guid back from the save. Discard existing guid.
        Nodes.Find(x=>x.entry).GUID = _containerCache.NodeLinks[0].BaseNodeGUID;

        foreach (var node in Nodes)
        {
            if (node.entry) {continue;}

            //removes edges that r connected to this node
            Edges.Where (x=>x.input.node==node).ToList().ForEach (edge=> _targetGraphView.RemoveElement(edge));

            //then remove the node
            _targetGraphView.RemoveElement(node);
            

        }
    }




}
