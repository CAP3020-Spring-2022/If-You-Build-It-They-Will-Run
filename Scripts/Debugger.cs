using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{

    [SerializeField] PlayerController playerController;
    /* float refSpeed;
    bool onGround; */
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.text = /* "Current Speed: " + playerController.GetCurrentSpeed() + */
                    "Stamina: " + Mathf.RoundToInt(playerController.GetPlayer().GetStamina()) +
                    "Momentum: " + Mathf.RoundToInt(playerController.GetPlayer().GetMomentum());
    }

    // Update is called once per frame
    void Update()
    {
        text.text = /* "Current Speed: " + playerController.GetCurrentSpeed() + */
                    "\nStamina: " + Mathf.RoundToInt(playerController.GetPlayer().GetStamina()) +
                    "\nMomentum: " + Mathf.RoundToInt(playerController.GetPlayer().GetMomentum());
    }
}
