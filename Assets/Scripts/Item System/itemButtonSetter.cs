using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemButtonSetter : MonoBehaviour
{
    public Item item;
    public GameObject container;


    public void SetItemButton(Item _item, string tooltipText,  Action buttonCallBack = null)
    {
        item = _item;

        container = transform.parent.gameObject;
        
        

        this.gameObject.name = item.itemName + " (ItemButton)";
        this.GetComponent<TooltipManager>().description = tooltipText;
        // this.GetComponent<TooltipManager>().tooltipPanel = MySceneManager.Instance.tooltipPanel;
        this.gameObject.GetComponent<TooltipManager>().detectChildren = true;
        // this.gameObject.GetComponent<TooltipManager>().tooltipText = tooltipText;
        // this.gameObject.GetComponent<TooltipManager>().cursorManager = cursorManager;
        this.gameObject.GetComponent<TooltipManager>().btn = this.gameObject;
        this.gameObject.GetComponent<TooltipManager>().tooltipType = TooltipManager.TooltipType.Item;
        this.gameObject.AddComponent<DragAndDropItem>();
        this.gameObject.GetComponentInChildren<Image>().sprite = item.sprite;
        
    

        Button button = this.GetComponent<Button>();

        button.onClick.AddListener(() => 
        buttonCallBack?.Invoke()
        );
    }
}
