using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    private Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = newItem.icon;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }

    public void destroySlot()
    {
        Destroy(gameObject);
    }
}
