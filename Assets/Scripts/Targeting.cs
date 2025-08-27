using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targeting : MonoBehaviour, IPointerClickHandler
{
    private Entity entity ;
    private RoundManager roundManager;


    void Start()
    {
        roundManager = RoundManager.Instance;

        entity = GetComponent<Entity>();
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ////////////////// fix the rest of the imlpementation\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        if (roundManager.currentPhase == RoundManager.TurnPhase.targetingATK)
        {
            roundManager.OnTargetSelected(entity); // call the method in RoundManager to set the target


        }
        if (roundManager.currentPhase == RoundManager.TurnPhase.targetingSKILL)
        {
            roundManager.OnTargetSelected(entity); // call the method in RoundManager to set the target


        }

    }


}
