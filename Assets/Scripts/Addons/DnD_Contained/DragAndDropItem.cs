using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    [HideInInspector] public Transform originalParent;
    [HideInInspector] public int originalIndex; // slot position in container

    public GameObject placeHolder;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        if (this.GetComponent<CanvasGroup>() == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        else canvasGroup = this.GetComponent<CanvasGroup>();


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CursorManager.Instance.isDragging = true;


        originalParent = transform.parent; // gets the original container
        originalIndex = transform.GetSiblingIndex();

        transform.SetParent(canvas.transform); // bring to top layer
        canvasGroup.blocksRaycasts = false;   // allow raycasts to go through
        canvasGroup.alpha = 0.8f;

        placeHolder = Instantiate(gameObject, originalParent);
        placeHolder.GetComponent<CanvasGroup>().blocksRaycasts = false;
        placeHolder.GetComponent<CanvasGroup>().alpha = 0.1f; 

        originalParent.GetComponent<DragAndDropContainer>().openSpaceForNewItem(placeHolder, eventData.position);
    }

public void OnDrag(PointerEventData eventData)
{
    rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    // Find the hovered container under the mouse
    var hoveredContainer = eventData.pointerEnter?.GetComponentInParent<DragAndDropContainer>();
    if (hoveredContainer != null)
    {
        // If placeholder is not in the hovered container, reparent it
        if (placeHolder.transform.parent != hoveredContainer.transform)
        {
            placeHolder.transform.SetParent(hoveredContainer.transform, false);
        }

        // Update placeholder position in the container
        hoveredContainer.openSpaceForNewItem(placeHolder, eventData.position);
    }
}

    public void OnEndDrag(PointerEventData eventData)
    {
        CursorManager.Instance.isDragging = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        var hoveredContainer = eventData.pointerEnter?.GetComponentInParent<DragAndDropContainer>();
        if (hoveredContainer == null)
        {

            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalIndex);
            originalParent.GetComponent<DragAndDropContainer>().openSpaceForNewItem(placeHolder, eventData.position);
        }
        else
        {
            transform.SetParent(placeHolder.transform.parent);
            transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
        }

        

        Destroy(placeHolder);
        
    }
}