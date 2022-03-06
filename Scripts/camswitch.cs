using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camswitch : MonoBehaviour
{

    public GameObject cam1;
    public GameObject cam2;
    public GameObject Player;
    public int counter = 0;

    // Update is called once per frame
    public async void HitButton()
    {

        if (counter % 2 == 0)
                {
                    cam1.SetActive(true);
                    cam2.SetActive(false);
                    counter++;
                    //Player.SetPositionAndRotation(new Vector3(170, 65, 0));
                }
        else if (counter % 2 == 1)
                {
                    cam2.SetActive(true);
                    cam1.SetActive(false);
                    counter++;
                    //Player.SetPositionAndRotation(new Vector3(170, 65, 0));
                }        
        
    }
}


