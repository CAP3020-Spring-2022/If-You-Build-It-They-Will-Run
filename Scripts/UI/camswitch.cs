using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camswitch : MonoBehaviour
{
    public bool buildphase;
    public GameObject cam1;
    public GameObject cam2;
    [SerializeField] PlayerController playerController; //TODO: Can use playerController? has transform to use SetPositionAndRoation()

    [SerializeField] Canvas staminaBar;
    [SerializeField] Canvas momentumBar;

    [SerializeField] Canvas buildUI;

    // public int counter = 0;

    public void Start()
    {
        buildphase = false;
        print("Hi");
        /* Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; */
    }

    // Update is called once per frame
    public void HitButton()
    {
        buildphase = !buildphase;
        if (buildphase) // build cam
                {
                    cam1.SetActive(true);
                    buildUI.gameObject.SetActive(true);

                    cam2.SetActive(false);
                    staminaBar.gameObject.SetActive(false);
                    momentumBar.gameObject.SetActive(false);
                    playerController.gameObject.SetActive(false);

               //     Cursor.lockState = CursorLockMode.None;
               
                    //Player.SetPositionAndRotation(new Vector3(170, 65, 0));
                }
        else if (!buildphase) // player cam
                {
                    cam1.SetActive(false);
                    buildUI.gameObject.SetActive(false);
                    
                    cam2.SetActive(true);
                    staminaBar.gameObject.SetActive(true);
                    momentumBar.gameObject.SetActive(true);
                    playerController.gameObject.SetActive(true);

                //    Cursor.lockState = CursorLockMode.Locked;
                  
                    //Player.SetPositionAndRotation(new Vector3(170, 65, 0));
                }
        Cursor.visible = true;
      //  Cursor.visible = buildphase;
      // // if(Cursor.visible)
      //  {
           // Cursor.lockState = CursorLockMode.Locked;
      //  }
      //  else
      //  {
       //     Cursor.lockState = CursorLockMode.None;
            //Cursor.lockState = CursorLockMode.Locked;
      //  }
        
    }
}


