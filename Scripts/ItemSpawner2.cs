using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner2 : MonoBehaviour
{
    public Button yourButton;
    public GameObject sampleObject;
    int xpos = -10;
    int ypos = 10;
    int zpos = 90;

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
    }

    public void AddObject()
    {
  
        GameObject newSpawn = Instantiate(sampleObject, new Vector3(xpos, ypos, zpos), sampleObject.transform.rotation);
    }
}
