using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items;

    /// <summary>
    /// 
    /// </summary>
    /// <returns> An instance of a random item</returns>
    public Item getRandom()
    {
        if (items.Count == 0) return null;

        int index = Random.Range(0, items.Count);
        return Instantiate(items[index]);
    }

    public Item GetItemByID(string itemID)
    {
        foreach (Item itm in items)
        {
            if (itm.itemID == itemID)
            {
                return Instantiate(itm);
            }
        }
        return null;
    }

        
    
}
