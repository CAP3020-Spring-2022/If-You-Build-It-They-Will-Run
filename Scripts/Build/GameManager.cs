using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum BlockType
    {
        BigLCrate,
        SmallLCrate,
        Pillar,
        Stair,
        ZCrate
    }

    int[] ItemQuantity;

    void Awake()
    {
        ItemQuantity[(int)BlockType.BigLCrate] = 10;
        ItemQuantity[(int)BlockType.SmallLCrate] = 10;
        ItemQuantity[(int)BlockType.Pillar] = 10;
        ItemQuantity[(int)BlockType.Stair] = 10;
        ItemQuantity[(int)BlockType.ZCrate] = 10;
    }
}
