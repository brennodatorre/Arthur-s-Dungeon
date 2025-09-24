using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this components makes the item smoothly move around, when item are being sorted while dragging around

[RequireComponent(typeof(RectTransform))]
public class DnD_SmoothSort : MonoBehaviour
{
    public float moveSpeed = 30f;

    private RectTransform rectTransform;
    private Vector3 targetPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetPosition = rectTransform.localPosition;
    }

    void Update()
    {
        // Smoothly move toward the last known target position
        if ((rectTransform.localPosition - targetPosition).sqrMagnitude > 0.01f)
        {
            rectTransform.localPosition = Vector3.Lerp(
                rectTransform.localPosition,
                targetPosition,
                Time.deltaTime * moveSpeed
            );
        }
    }

    void LateUpdate()
    {
        // Layout groups finish positioning in LateUpdate.
        // Cache where the layout wants this element to be.
        targetPosition = rectTransform.localPosition;
    }
}
