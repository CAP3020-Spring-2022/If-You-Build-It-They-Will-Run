using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killPlane : MonoBehaviour
{
    public GameObject Player;
    public Vector3 spawn = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        Player.transform.SetPositionAndRotation(spawn, Quaternion.identity);
    }
}
