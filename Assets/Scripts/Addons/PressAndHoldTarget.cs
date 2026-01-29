using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PressAndHoldTarget : MonoBehaviour, IPointerDownHandler
{

    private Coroutine shakeCoroutine;

    public static PressAndHoldTarget target;

    

    [SerializeField] private bool wasCompleted = false;
    public bool isWaiting = false; // lets object be PAHT targeted
    


    public void OnPointerDown(PointerEventData eventData)
    {

        if (!isWaiting ) { return; }
        shakeCoroutine = AnimationManager.Instance.doShakeAnimation(gameObject, CursorManager.Instance.pahtDuration);
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
            target.StopCoroutine(target.shakeCoroutine);
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
