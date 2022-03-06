using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerData;

public class HealthBar : MonoBehaviour
{
    Slider healthBar;
    /* public Health playerHealth; */
    // player data struct can handle this data without another
    Player player;

    void Start()
    {
        /* playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>(); */
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GetPlayer();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = 100.0f;
        healthBar.value = 100.0f;
    }

    void Update() {
        SetHealth(player.GetStamina());
    }

    void SetHealth(float hp)
    {
        healthBar.value = hp;
    }
}

