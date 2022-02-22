using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Item> itemList = new List<Item>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Inventory.instance.addItem(itemList[Random.Range(0, itemList.Count)]);
        }
    }
}
