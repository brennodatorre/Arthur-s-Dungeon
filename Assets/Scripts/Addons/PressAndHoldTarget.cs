using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressAndHoldTarget : MonoBehaviour
{

    public static PressAndHoldTarget target;

    public void StartHold()
    {
        target = this;
        print("started hold");
    }

    public static void StopHoldGlobal()
    {
        if (target != null)
        {
            target = null;
            CursorManager.Instance.stopPAHTHolding();
        }
    }
}
