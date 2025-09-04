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

    [HideInInspector] public TextMeshProUGUI tooltipText;
    [HideInInspector] public CursorManager cursorManager;
    [HideInInspector] public Entity entity;
    private RectTransform tooltipRect;
    public Canvas canvas;
    private RectTransform canvasRect;


    public GameObject tooltipPanel;
    public GameObject btn;

    [Space(10)]
    public TooltipType tooltipType = TooltipType.None;
    public Vector3 offset = new Vector3(0, 0, 0);
    public bool displayToolTip = true;
    public bool detectChildren = true;


    public string description;


    private bool isTooltipShowing = false;



    private Coroutine pointerCoroutine;


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

        tooltipText = tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>(); ;
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!detectChildren && eventData.pointerEnter != gameObject) return; // Ignore if it's actually a child being hovered

        
        
        // blocks tooltip
        if (displayToolTip == false) { return; }

        StartCoroutine(AnimateTooltipOpen());

        setText();


        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        Vector2 tooltipSize = tooltipRect.sizeDelta;

        // Convert screen position to local position in canvas
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, canvas.worldCamera, out pos);

        // Clamp tooltip inside the canvas
        float pivotX = Mathf.Clamp(pos.x, -canvasRect.rect.width / 2 + tooltipSize.x / 2, canvasRect.rect.width / 2 - tooltipSize.x / 2);
        float pivotY = Mathf.Clamp(pos.y, -canvasRect.rect.height / 2 + tooltipSize.y / 2, canvasRect.rect.height / 2 - tooltipSize.y / 2);

        tooltipRect.anchoredPosition = new Vector2(pivotX, pivotY);

        pointerCoroutine = StartCoroutine(updateText());

        isTooltipShowing = true;
        


    }




    public void OnPointerExit(PointerEventData eventData)
    {
        if (!detectChildren &&eventData.pointerEnter != gameObject) return; // Ignore if it's actually a child being hovered

        
        
        // blocks tooltip
        if (displayToolTip == false) { return; }

        tooltipPanel.SetActive(false);

        if (pointerCoroutine != null) { StopCoroutine(pointerCoroutine); }

        isTooltipShowing = false;
        

    }

    // Updates the tooltip text based on the current description
    // in real-time, so that the tooltip can be update while the player is hovering over it
    private IEnumerator updateText()
    {


        while (tooltipPanel.activeSelf)
        {
            setText();

            yield return new WaitForSeconds(0.1f); // Update every 0.1 seconds
        }
    }

    private void setText()
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
    }

    public IEnumerator AnimateTooltipOpen()
    {
        tooltipPanel.SetActive(true);
        tooltipPanel.transform.localScale = Vector3.zero;

        float time = 0f;
        float duration = 0.2f; // how fast the animation is
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            // ease out for a nice pop
            float scale = Mathf.SmoothStep(0f, 1f, t);
            tooltipPanel.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        tooltipPanel.transform.localScale = Vector3.one;
    }

    


}
