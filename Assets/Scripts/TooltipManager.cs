using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2 screenBounds = new Vector2(Screen.width, Screen.height);


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
        if (isEntity && entity.entityType == Entity.EntityType.Enemy) {
            return;
        } 

        tooltipPanel.SetActive(true);
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        Canvas canvas = tooltipPanel.GetComponentInParent<Canvas>();

        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        Vector2 tooltipSize = tooltipRect.sizeDelta;

        // Convert screen position to local position in canvas
        Vector2 localPoint;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, canvas.worldCamera, out localPoint);

        // Clamp tooltip inside the canvas
        float pivotX = Mathf.Clamp(localPoint.x, -canvasRect.rect.width / 2 + tooltipSize.x / 2, canvasRect.rect.width / 2 - tooltipSize.x / 2);
        float pivotY = Mathf.Clamp(localPoint.y, -canvasRect.rect.height / 2 + tooltipSize.y / 2, canvasRect.rect.height / 2 - tooltipSize.y / 2);

        tooltipRect.localPosition = new Vector2(pivotX, pivotY);

        // Set text
        if (isEntity && entity.entityType == Entity.EntityType.Player)
        {
            tooltipText.text = entity.getStatusAsString();
        }
        else if (hasDescription)
        {
            tooltipText.text = description;
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);  
 
    }
}
