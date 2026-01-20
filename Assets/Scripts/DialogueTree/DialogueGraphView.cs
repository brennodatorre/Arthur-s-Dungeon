// using System.Collections.Generic;
// using System.Linq;

// using UnityEditor;
// using UnityEditor.Experimental.GraphView;
// using UnityEngine;
// using UnityEngine.UIElements;

// public class DialogueGraphView : GraphView
// {
//     private DialogueGraph currentGraph;

//     public DialogueGraphView()
//     {
//         style.flexGrow = 1;

//         // Background grid
//         var grid = new GridBackground();
//         Insert(0, grid);
//         grid.StretchToParentSize();

//         // Graph interaction tools
//         this.AddManipulator(new ContentZoomer());
//         this.AddManipulator(new ContentDragger());
//         this.AddManipulator(new SelectionDragger());
//         this.AddManipulator(new RectangleSelector());
//     }

//     public void CreateNode(string nodeName)
//     {
//         var node = new DialogueNodeView(System.Guid.NewGuid().ToString(), "New dialogue line");
//         node.title = nodeName;
//         node.SetPosition(new Rect(100, 200, 250, 150));
//         AddElement(node);
//     }

//     //Ensure ports only connect to valid ones
//     public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
//     {
//         return ports.ToList().Where(port =>
//             startPort != port &&
//             startPort.node != port.node &&
//             startPort.direction != port.direction // Prevent output-to-output or input-to-input
//         ).ToList();
//     }

//     public void LoadGraph(DialogueGraph graph)
//     {
//         currentGraph = graph;
//         ClearGraph();

//         // Recreate nodes
//         foreach (var nodeData in graph.nodes)
//         {
//             // Create the node view with saved data
//             var nodeView = new DialogueNodeView(nodeData.GUID, nodeData.dialogue, nodeData.image);

//             // Assign the saved conditions list directly
//             nodeView.conditions = nodeData.conditions != null
//                 ? new List<DialogueCondition>(nodeData.conditions)
//                 : new List<DialogueCondition>();

//             nodeView.SetPosition(nodeData.nodePosition);
//             nodeView.RefreshConditionsUI();
//             AddElement(nodeView);
//         }

//         // Recreate edges (connections)
//         foreach (var nodeData in graph.nodes)
//         {
//             var parentView = nodes.ToList().OfType<DialogueNodeView>().FirstOrDefault(x => x.GUID == nodeData.GUID);
//             if (parentView == null) continue;

//             foreach (var childGUID in nodeData.options)
//             {
//                 var childView = nodes.ToList().OfType<DialogueNodeView>().FirstOrDefault(x => x.GUID == childGUID);
//                 if (childView == null) continue;

//                 var parentPort = parentView.outputContainer[0] as Port;
//                 var childPort = childView.inputContainer[0] as Port;

//                 if (parentPort != null && childPort != null)
//                 {
//                     var edge = parentPort.ConnectTo(childPort);

//                     AddElement(edge);
//                 }
//             }
//         }
//     }

// public void SaveGraph()
// {
//     if (currentGraph == null) return;

//     // Clear old nodes in the asset
//     foreach (var oldNode in currentGraph.nodes)
//         Object.DestroyImmediate(oldNode, true);

//     currentGraph.nodes.Clear();

//         // Save all nodes
//         foreach (var node in nodes.ToList().OfType<DialogueNodeView>())
//         {
//             var nodeData = ScriptableObject.CreateInstance<DialogueNode>();
//             nodeData.GUID = node.GUID;
//             nodeData.dialogue = node.dialogueText;
//             nodeData.nodePosition = node.GetPosition();
//             nodeData.image = node.nodeSprite;

//             nodeData.conditions.Clear();
//             nodeData.conditions = node.conditions != null
//                 ? node.conditions.Select(c => new DialogueCondition {
//                     key = c.key,
//                     expectedValue = c.expectedValue,
//                     type = c.type
//                 }).ToList()
//                 : new List<DialogueCondition>();

//         // Save outgoing connections
//         var connections = edges.Where(e => e.output.node == node).ToList();
//         nodeData.options = connections
//             .Select(c => ((DialogueNodeView)c.input.node).GUID)
//             .ToList();

//         // Save incoming connection (parent)
//         var parentConnection = edges.FirstOrDefault(e => e.input.node == node);
//         nodeData.previousDialogue = parentConnection != null
//             ? ((DialogueNodeView)parentConnection.output.node).GUID
//             : null;

//         // Add as sub-asset
//         AssetDatabase.AddObjectToAsset(nodeData, currentGraph);
//         currentGraph.nodes.Add(nodeData);

//         EditorUtility.SetDirty(nodeData);
//     }

//     EditorUtility.SetDirty(currentGraph);
//     AssetDatabase.SaveAssets();
// }


//     private void ClearGraph()
//     {
//         // Remove edges first, then nodes
//         foreach (var edge in edges.ToList())
//         {
//             RemoveElement(edge);
//         }
//         foreach (var node in nodes.ToList())
//         {
//             RemoveElement(node);
//         }
//     }

//     private DialogueNodeView CreateNodeView(DialogueNode nodeData)
//     {
//         var nodeView = new DialogueNodeView(nodeData.GUID, nodeData.dialogue);

//         nodeView.title = "Dialogue";

//         nodeView.SetPosition(new Rect(100, 200, 250, 150));
//         return nodeView;
//     }
// }
