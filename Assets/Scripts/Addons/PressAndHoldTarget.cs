using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PressAndHoldTarget : MonoBehaviour, IPointerDownHandler
{

    public static PressAndHoldTarget target;

    [SerializeField] private bool wasCompleted = false;


    public void OnPointerDown(PointerEventData eventData)
    {
        
        target = this;
        CursorManager.Instance.holdable = target;
        CursorManager.Instance.holdableMEM = target;
        CursorManager.Instance.startPAHTHolding();
        print("started hold");
    }

    public static void StopHoldGlobal()
    {
        if (target != null)
        {
            target = null;
            CursorManager.Instance.holdable = null;
            CursorManager.Instance.stopPAHTHolding();
            print("stoped hold");
        }
    }

    public bool askIfPAHTWasCompleted()
    {
        if (wasCompleted)
        {
            wasCompleted = false;
            return true;
        }
        else { return false; }
    }

    public void gotCompleted()
    {
        wasCompleted = true;
    }

}
