using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camswitch : MonoBehaviour
{

    public GameObject cam1;
    public GameObject cam2;
    public GameObject Player; //TODO: Can use playerController? has transform to use SetPositionAndRoation()

    [SerializeField] Canvas staminaBar;
    [SerializeField] Canvas momentumBar;

    [SerializeField] Canvas buildUI;

    public int counter = 0;

    // Update is called once per frame
    public void HitButton()
    {

        if (counter % 2 == 0) // build cam
                {
                    cam1.SetActive(true);
                    cam2.SetActive(false);
                    staminaBar.gameObject.SetActive(false);
                    momentumBar.gameObject.SetActive(false);
                    buildUI.gameObject.SetActive(true);
                    counter++;
                    //Player.SetPositionAndRotation(new Vector3(170, 65, 0));
                }
        else if (counter % 2 == 1) // player cam
                {
                    cam2.SetActive(true);
                    cam1.SetActive(false);
                    staminaBar.gameObject.SetActive(true);
                    momentumBar.gameObject.SetActive(true);
                    buildUI.gameObject.SetActive(false);
                    counter++;
                    //Player.SetPositionAndRotation(new Vector3(170, 65, 0));
                }        
        
    }
}


