using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner2 : MonoBehaviour
{
    public Button yourButton;
    public GameObject sampleObject;
    int xpos = -9;
    int ypos = 10;
    int zpos = 91;

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
    }

    public void AddObject()
    {
        GameObject newSpawn = Instantiate(sampleObject, new Vector3(xpos + 10, ypos, zpos), Quaternion.identity);
        newSpawn.GetComponent<Renderer>().material.color = Color.red;
        xpos += 10;
    }
}
