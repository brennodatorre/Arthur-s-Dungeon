using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DialogueCondition;

public class DialogueManager : MonoBehaviour
{
    public DialogueGraph dialogueGraph;
    private Dictionary<string, DialogueNode> nodeLookup;
    private DialogueNode currentNode;

    // Example of game state
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();
    private HashSet<string> items = new HashSet<string>();

    void Start()
    {
        nodeLookup = dialogueGraph.nodes.ToDictionary(n => n.GUID, n => n);
        currentNode = dialogueGraph.nodes.FirstOrDefault(n => string.IsNullOrEmpty(n.previousDialogue));
        ShowCurrentNode();
    }

    void ShowCurrentNode()
    {
        if (currentNode == null) return;

        Debug.Log("Dialogue: " + currentNode.dialogue);

        // Filter options based on conditions
        var availableOptions = currentNode.options
            .Select(guid => nodeLookup[guid])
            .Where(node => AreConditionsMet(node))
            .ToList();

        if (availableOptions.Count > 0)
        {
            Debug.Log("Options:");
            for (int i = 0; i < availableOptions.Count; i++)
            {
                Debug.Log($"{i + 1}: {availableOptions[i].dialogue}");
            }
        }
        else
        {
            Debug.Log("End of dialogue branch.");
        }
    }

    bool AreConditionsMet(DialogueNode node)
    {
        foreach (var cond in node.conditions)
        {
            switch (cond.type)
            {
                case ConditionType.HasItem:
                    if (!items.Contains(cond.key)) return false;
                    break;
                case ConditionType.FlagTrue:
                    if (!flags.ContainsKey(cond.key) || flags[cond.key] != cond.expectedValue)
                        return false;
                    break;
                case ConditionType.QuestCompleted:
                    // Implement your quest check here
                    break;
                case ConditionType.None:
                    
                    break;
            }
        }
        return true;
    }

    public void ChooseOption(int index)
    {
        var availableOptions = currentNode.options
            .Select(guid => nodeLookup[guid])
            .Where(node => AreConditionsMet(node))
            .ToList();

        if (index < 0 || index >= availableOptions.Count) return;

        currentNode = availableOptions[index];
        ShowCurrentNode();
    }

    // Example helper functions
    public void GiveItem(string item) => items.Add(item);
    public void SetFlag(string key, bool value) => flags[key] = value;
}

