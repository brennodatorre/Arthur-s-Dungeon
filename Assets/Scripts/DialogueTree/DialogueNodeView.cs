using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class DialogueNodeView : Node
{
    public string GUID;
    public string dialogueText;
    public Sprite nodeSprite;
    public List<DialogueCondition> conditions = new List<DialogueCondition>();

    private VisualElement conditionsContainer;

    public DialogueNodeView(string guid, string dialogue, Sprite sprite = null, List<DialogueCondition> conds = null)
    {
        GUID = guid;
        dialogueText = dialogue;
        nodeSprite = sprite;
        conditions = conds ?? new List<DialogueCondition>();

        // Input/output ports
        var input = CreatePort("Previous", Direction.Input, Port.Capacity.Single);
        inputContainer.Add(input);

        var output = CreatePort("Options", Direction.Output, Port.Capacity.Multi);
        outputContainer.Add(output);

        // Dialogue text
        var textField = new TextField("Dialogue:");
        textField.value = dialogueText;
        textField.RegisterValueChangedCallback(evt => dialogueText = evt.newValue);
        mainContainer.Add(textField);

        // Sprite picker
        var spriteField = new ObjectField("Sprite");
        spriteField.objectType = typeof(Sprite);
        spriteField.value = nodeSprite;
        spriteField.RegisterValueChangedCallback(evt => nodeSprite = (Sprite)evt.newValue);
        mainContainer.Add(spriteField);

        // Conditions container
        conditionsContainer = new VisualElement();
        conditionsContainer.style.flexDirection = FlexDirection.Column;
        mainContainer.Add(conditionsContainer);

        // Add condition button
        var addConditionBtn = new Button(() =>
        {
            var newCond = new DialogueCondition();
            conditions.Add(newCond);
            AddConditionUI(newCond);
        });
        addConditionBtn.text = "Add Condition";
        mainContainer.Add(addConditionBtn);

        // Populate existing conditions
        foreach (var cond in conditions)
        {
            AddConditionUI(cond);
        }

        RefreshExpandedState();
        RefreshPorts();
    }

    private void AddConditionUI(DialogueCondition cond)
    {
        var condContainer = new VisualElement();
        condContainer.style.flexDirection = FlexDirection.Row;

        // Type dropdown
        var typeField = new EnumField(cond.type);
        typeField.RegisterValueChangedCallback(evt => cond.type = (DialogueCondition.ConditionType)evt.newValue);
        condContainer.Add(typeField);

        // Key text field
        var keyField = new TextField();
        keyField.value = cond.key;
        keyField.RegisterValueChangedCallback(evt => cond.key = evt.newValue);
        condContainer.Add(keyField);

        // Expected value toggle
        var toggle = new Toggle();
        toggle.value = cond.expectedValue;
        toggle.RegisterValueChangedCallback(evt => cond.expectedValue = evt.newValue);
        condContainer.Add(toggle);

        // Delete button
        var deleteBtn = new Button(() =>
        {
            conditions.Remove(cond);          // remove from the list
            conditionsContainer.Remove(condContainer); // remove from UI
        });
        deleteBtn.text = "X";
        condContainer.Add(deleteBtn);

        conditionsContainer.Add(condContainer);
    }

    public void RefreshConditionsUI()
    {
        conditionsContainer.Clear();
        foreach (var cond in conditions)
        {
            AddConditionUI(cond);
        }
    }

    private Port CreatePort(string portName, Direction direction, Port.Capacity capacity)
    {
        var port = InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(bool));
        port.portName = portName;
        return port;
    }
}
