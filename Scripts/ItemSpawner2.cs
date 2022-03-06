using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner2 : MonoBehaviour
{
    public Button yourButton;
    public GameObject sampleObject;
    public KeyCode triggerKey;

    ItemBehavior itemBehavior;

    int xpos = -10;
    int ypos = 10;
    int zpos = 90;

    void Awake()
    {
        yourButton = GetComponent<Button>();
        itemBehavior = GetComponent<ItemBehavior>();
    }

    void Update()
    {
        if(Input.GetKeyDown(triggerKey))
        {
            yourButton.onClick.Invoke();
        }
    }
    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
    }

    public void AddObject()
    {
        if (itemBehavior.quantity > 0)
        {
            GameObject newSpawn = Instantiate(sampleObject, new Vector3(xpos, ypos, zpos), sampleObject.transform.rotation);
            itemBehavior.quantity--;
        }
        else
        {
            Debug.Log("All of this item has been used!");
        }   
    }
}
