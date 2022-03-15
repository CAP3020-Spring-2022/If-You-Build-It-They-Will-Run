using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCollider : MonoBehaviour
{
    bool collide = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            collide = true;
            Debug.Log("trigger is set to true");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.name == "Player")
        {
            collide = false;
        }
    }
}
