using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropContainer : MonoBehaviour, IDropHandler
{
    public int numberOfBackgroundUIElements = 0; // non-draggable prefix items
    public float animateTime = 0.12f; // animation duration
    Coroutine reorderCoroutine;

    public bool acceptsOutsideItems = false;

    public void OnDrop(PointerEventData eventData)
    {
        

        var item = eventData.pointerDrag?.GetComponent<DragAndDropItem>();
        if (item == null) return;

        

        // Reparent into this container (keeps world position)
        if (acceptsOutsideItems || item.transform.parent == transform) 
        {
            item.transform.SetParent(transform, true);

            openSpaceForNewItem(item.placeHolder.transform.gameObject, eventData.position);
        }
    }

    public void openSpaceForNewItem(GameObject placeholder, Vector3 pointerPosition)
    {
        // compute target index for grid cell under pointer
        RectTransform gridRect = GetComponent<RectTransform>();
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        if (grid == null) return;

        // Convert pointer position to local coordinates
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, pointerPosition, Camera.main, out localPoint);

        // columns calculation (safe)
        int columns = Mathf.Max(1, Mathf.FloorToInt((gridRect.rect.width + grid.spacing.x) / (grid.cellSize.x + grid.spacing.x)));

        // center-based column/row (adjust to center)
        int col = Mathf.FloorToInt((localPoint.x + gridRect.rect.width * 0.5f) / (grid.cellSize.x + grid.spacing.x) + 0.5f);
        int row = Mathf.FloorToInt((gridRect.rect.height * 0.5f - localPoint.y) / (grid.cellSize.y + grid.spacing.y) + 0.5f);

        int newIndex = row * columns + col;

        // clamp
        newIndex = Mathf.Clamp(newIndex, numberOfBackgroundUIElements, transform.childCount);

        // if placeholder already at that index -> nothing to do
        if (placeholder.transform.GetSiblingIndex() == newIndex) return;

        // start (or restart) coroutine to animate reorder
        if (reorderCoroutine != null) StopCoroutine(reorderCoroutine);
        reorderCoroutine = StartCoroutine(AnimateReorder(placeholder, newIndex, gridRect, grid));
    }

    IEnumerator AnimateReorder(GameObject placeholder, int newIndex, RectTransform containerRect, GridLayoutGroup grid)
    {
        // Ensure layout is enabled so we can compute new positions
        grid.enabled = true;

        // collect children rects (all children - placeholder will be included)
        var childRects = new List<RectTransform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            childRects.Add(transform.GetChild(i) as RectTransform);
        }

        // record old positions (localPosition) for all children
        var oldPos = new Dictionary<RectTransform, Vector3>();
        foreach (var ch in childRects)
        {
            oldPos[ch] = ch.localPosition;
        }

        // move placeholder to new index so layout computes "target" positions
        placeholder.transform.SetSiblingIndex(newIndex);

        // force immediate rebuild so layout writes new positions now
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);

        // collect new positions (after layout has placed them)
        var newPos = new Dictionary<RectTransform, Vector3>();
        childRects.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var r = transform.GetChild(i) as RectTransform;
            childRects.Add(r);
            newPos[r] = r.localPosition;
        }

        // disable layout so it doesn't keep snapping positions while we animate
        grid.enabled = false;

        // revert children back to their old positions (visual stays the same as before change)
        foreach (var kv in oldPos)
        {
            // if this child still exists (safety)
            if (kv.Key != null)
                kv.Key.localPosition = kv.Value;
        }

        // animate from old -> new for every child except the placeholder itself (make placeholder stay put)
        float elapsed = 0f;
        // create lists of children to animate (exclude placeholder object)
        var animChildren = new List<RectTransform>();
        foreach (var ch in childRects)
        {
            if (ch == placeholder.transform) continue; // don't animate placeholder (it's the ghost)
            // ensure we have both positions recorded
            if (!oldPos.ContainsKey(ch) || !newPos.ContainsKey(ch)) continue;
            animChildren.Add(ch);
        }

        while (elapsed < animateTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / animateTime));
            foreach (var ch in animChildren)
            {
                Vector3 a = oldPos[ch];
                Vector3 b = newPos[ch];
                ch.localPosition = Vector3.LerpUnclamped(a, b, t);
            }
            yield return null;
        }

        // finalize: ensure all are at their exact new positions
        foreach (var ch in animChildren)
        {
            ch.localPosition = newPos[ch];
        }

        // re-enable layout and force one final rebuild so everything is clean
        grid.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);

        
        reorderCoroutine = null;

        
    }


       
}