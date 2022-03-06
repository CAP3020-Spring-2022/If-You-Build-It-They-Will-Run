using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    GameObject gameManager;
    public int quantity;

    void Awake()
    {
        quantity = 10;
    }
}
