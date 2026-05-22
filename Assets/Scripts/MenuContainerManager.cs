using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MenuContainerManager : MonoBehaviour
{
    public GameObject defaultItem;

     
    

    void Start()
    {
        defaultItem.SetActive(false);
    }

    void Update()
    {
        ///testing
        // if (Input.GetKeyDown(KeyCode.Z))
        // {
        //     AddItem(defaultItem.GetComponentInChildren<Image>(), "Test Item", () => Debug.Log("Item Clicked!"));
        // }   
    }


    public GameObject AddItem(string itemName, Action onClick)
    {
        return AddItem(defaultItem.GetComponentInChildren<Image>().sprite, itemName, onClick);
    }

    public GameObject AddItem(Sprite icon, string itemName, Action onClick)
    {
            var item = Instantiate(defaultItem, defaultItem.transform.parent);
            item.SetActive(true);

            item.transform.GetChild(1).GetComponent<Image>().sprite= icon;
            item.GetComponentInChildren<TextMeshProUGUI>().text = itemName;
            item.GetComponent<Button>().onClick.AddListener(() => onClick());

            return item;


    }


    public void removeItem(GameObject item)
    {
        Destroy(item);
    }
}
