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
        // Forward vector to see where the camera is facing
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camCoords = Camera.main.transform.position;
        Vector3 itemCoords = new Vector3(0, 0, 0);
        print(camForward);
        // new Vector3(camCoords.x, camCoords.y + 5, camCoords.z + 5);
        // Case where all coords are +
        if ((camForward.x >= 0.0 || camForward.x == 0.0) && (camForward.y >= 0.0 || camForward.y == 0) && (camForward.z >= 0.0 || camForward.z == 0.0)) {
            itemCoords = new Vector3(camCoords.x + 15, camCoords.y, camCoords.z + 10);
        }
        // Case where only x is -, y and z +
        else if (camForward.x < 0 && camForward.y >= 0 && camForward.z >= 0)
        {
            itemCoords = new Vector3(camCoords.x - 15, camCoords.y, camCoords.z + 10);
        }
        // Case where x and y -, z +
        else if (camForward.x < 0 && camForward.y < 0 && camForward.z >= 0)
        {
            itemCoords = new Vector3(camCoords.x - 15, camCoords.y, camCoords.z + 10);
        }
        // Case where all components -
       else if(camForward.x < 0 && camForward.y < 0 && camForward.z < 0)
       {
            itemCoords = new Vector3(camCoords.x - 15, camCoords.y, camCoords.z - 10);
        }
        else
        {
            itemCoords = new Vector3(camCoords.x + 15, camCoords.y, camCoords.z + 10);
        }
        if (itemBehavior.quantity > 0)
        {
            GameObject newSpawn = Instantiate(sampleObject, itemCoords, sampleObject.transform.rotation);
            itemBehavior.quantity--;
        }
        else
        {
            Debug.Log("All of this item has been used!");
        }   
    }
}
