using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCondition 
{
    public enum ConditionType
    {
        HasItem,
        QuestCompleted,
        FlagTrue,
        None
    }

    public ConditionType type = ConditionType.None;
    [TextArea(2, 5)]
    public string key = ""; // e.g., "SwordObtained", "Quest_1", "Flag_ABC"
    public bool expectedValue = false; // For flags

}
