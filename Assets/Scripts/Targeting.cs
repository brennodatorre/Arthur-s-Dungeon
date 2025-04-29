using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targeting : MonoBehaviour, IPointerClickHandler
{
    private Entity entity ;
    public RoundManager roundManager;



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

    // Start is called before the first frame update
    void Start()
    {
       entity = GetComponent<Entity>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
