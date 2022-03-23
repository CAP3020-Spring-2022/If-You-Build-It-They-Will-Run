using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerData;

// Naming is really backwards on these, oops...
public class StamBar : MonoBehaviour
{
    Slider stamBar;
    /* public Stam playerStam; */
    Player player;

    private void Start()
    {
        /* playerStam = GameObject.FindGameObjectWithTag("Player").GetComponent<Stam>(); */
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GetPlayer();
        stamBar = GetComponent<Slider>();
        stamBar.maxValue = 100.0f;
        stamBar.value = 100.0f;
    }

    void Update() {
        SetStam(player.GetMomentum());
    }

    void SetStam(float hp)
    {
        stamBar.value = hp;
    }
}

