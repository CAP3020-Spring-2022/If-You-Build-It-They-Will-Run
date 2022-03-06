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
    }

    // Update is called once per frame
    void Update()
    {
        text.text = /* "Current Speed: " + playerController.GetCurrentSpeed() + */
                    "Stamina: " + Mathf.RoundToInt(playerController.GetPlayer().GetStamina()) +
                    "\nMomentum: " + Mathf.RoundToInt(playerController.GetPlayer().GetMomentum());
        text.text += "\nAction: " + playerController.GetPlayer().GetAction();
        text.text += "\nCurrent Speed: " + playerController.GetPlayer().GetSpeed();
    }
}
