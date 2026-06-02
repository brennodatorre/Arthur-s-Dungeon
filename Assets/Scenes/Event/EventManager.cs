using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void InputSkill(Skill skill)
    {
        // base start method for all events, can be overridden by child classes
    }
    public virtual void InputItem(Item item)
    {
        // base start method for all events, can be overridden by child classes
    }

}
