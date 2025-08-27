using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2 screenBounds = new Vector2(Screen.width, Screen.height);

    public enum TooltipType
    {
        None,
        Entity,
        Skill,
        UIElement
    }


    public string description;
    public GameObject tooltipPanel;
    [HideInInspector] public TextMeshProUGUI tooltipText;
    [HideInInspector] public CursorManager cursorManager;
    public GameObject btn;
    public Vector3 offset = new Vector3(0, 0, 0);

    public TooltipType tooltipType = TooltipType.None;

    private Coroutine pointerCoroutine;


    public Entity entity;

    void Awake()
    {

        if (GetComponent<Entity>() != null)
        {
            tooltipType = TooltipType.Entity;
            entity = GetComponent<Entity>();
        }



    }

    void Start()
    {
        if (tooltipPanel == null) { tooltipPanel = ButtonManager.Instance.tooltipPanel; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipText = tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();

        if (tooltipType == TooltipType.Entity && entity.entityType == Entity.EntityType.Enemy)
        {
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

        pointerCoroutine = StartCoroutine(updateText());


    }




    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);

        if (pointerCoroutine != null) { StopCoroutine(pointerCoroutine); }

    }

    // Updates the tooltip text based on the current description
    // in real-time, so that the tooltip can be update while the player is hovering over it
    private IEnumerator updateText()
    {
        

        while (tooltipPanel.activeSelf)
        {
            // Set text
            if (tooltipType == TooltipType.Entity && entity.entityType == Entity.EntityType.Player)
            {
                tooltipText.text = entity.getStatusAsString();
            }
            else if (tooltipType == TooltipType.Skill)
            {
                tooltipText.text = description;
            }
            else if (tooltipType == TooltipType.UIElement)
            {
                tooltipText.text = description;
            }
            else
            {
                tooltipText.text = "No description available.";
            }

            yield return new WaitForSeconds(0.1f); // Update every 0.1 seconds
        }
    }
}
