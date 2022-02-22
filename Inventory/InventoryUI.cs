using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private List<ItemSlot> slotList = new List<ItemSlot>();
    public GameObject itemSlotPrefab;
    public Transform inventoryItemTransform;
    public int numberOfSlots => gameObject.transform.childCount;

    KeyCode[] selectionKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };

    private void UpdateInventoryUI()
    {
        int itemCount = Inventory.instance.itemList.Count;
        if (itemCount > slotList.Count)
        {
            addItemSlots(itemCount);
        }

        for (int i = 0; i < slotList.Count; i++)
        {
            if (i <= itemCount)
            {
                slotList[i].AddItem(Inventory.instance.itemList[i]);
            }
            else
            {
                slotList[i].destroySlot();
                slotList.RemoveAt(i);
            }
        }
    }

    private void addItemSlots(int itemCount)
    {
        int amt = itemCount - slotList.Count;
        for (int i = 0; i < amt; i++)
        {
            GameObject gameobj = Instantiate(itemSlotPrefab, inventoryItemTransform);
            ItemSlot newSlot = gameobj.GetComponent<ItemSlot>();
            slotList.Add(newSlot);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Inventory.instance.onItemChange += UpdateInventoryUI;
        UpdateInventoryUI();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < selectionKeys.Length; i++)
        {
            if (Input.GetKeyDown(selectionKeys[i]))
            {
                Debug.Log("Select Item: " + i);
                if(i + 1 <= slotList.Count)
                {
                    slotList[i].UseItem();
                }
                return;
            }
        }
    }
}
