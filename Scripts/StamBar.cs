using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StamBar : MonoBehaviour
{
    public Slider stamBar;
    public Stam playerStam;

    private void Start()
    {
        playerStam = GameObject.FindGameObjectWithTag("Player").GetComponent<Stam>();
        stamBar = GetComponent<Slider>();
        stamBar.maxValue = playerStam.maxStam;
        stamBar.value = playerStam.maxStam;
    }

    public void SetStam(int hp)
    {
        stamBar.value = hp;
    }
}

