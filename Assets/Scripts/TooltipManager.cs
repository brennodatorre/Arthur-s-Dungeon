using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    [HideInInspector]public CursorManager cursorManager;
    public GameObject btn ;
    public Vector3 offset = new Vector3(0, 0, 0);

    public bool hasDescription = false;
    public bool isEntity = false;
    public Entity entity;

    void Start()
    {
        if (GetComponent<Entity>() != null) {
            isEntity = true;
            entity = GetComponent<Entity>();
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {


        if (isEntity && entity.entityType == Entity.EntityType.Player){
            
            tooltipText.text = entity.getStatusAsString();
            tooltipPanel.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipPanel.GetComponent<RectTransform>());
            tooltipPanel.GetComponent<RectTransform>().position = 
                entity.transform.position ;

        }
        else if (hasDescription){
            tooltipText.text = description;
            tooltipPanel.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipPanel.GetComponent<RectTransform>());
            tooltipPanel.GetComponent<RectTransform>().position = 
                btn.transform.position + offset;
        }


       

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);  
 
    }
}
