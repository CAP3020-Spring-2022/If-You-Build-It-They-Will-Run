using UnityEngine;
using UnityEngine.UI;
using PlayerData;

// Naming is really backwards on these, oops...
public class MomentumBar : MonoBehaviour
{
    Slider bar;
    Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GetPlayer();
        bar = GetComponent<Slider>();
        bar.maxValue = 2.0f;
        bar.value = 0.0f;
    }

    void Update() {
        SetStam(player.momentum);
    }

    void SetStam(float hp)
    {
        bar.value = hp;
    }
}

