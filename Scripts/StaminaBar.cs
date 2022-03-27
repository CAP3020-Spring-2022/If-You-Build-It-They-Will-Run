using UnityEngine;
using UnityEngine.UI;
using PlayerData;

public class StaminaBar : MonoBehaviour
{
    Slider staminaBar;
    Player player;

    void Start()
    {
        /* playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>(); */
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GetPlayer();
        staminaBar = GetComponent<Slider>();
        staminaBar.maxValue = 100.0f;
        staminaBar.value = 100.0f;
    }

    void Update() {
        SetHealth(player.stamina);
    }

    void SetHealth(float hp)
    {
        staminaBar.value = hp;
    }
}

