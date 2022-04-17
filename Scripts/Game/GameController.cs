// A8 also used in all previous assignments
// GameController.cs by Erin Hu
// Description: Escapes the program

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerData;

public class GameController : MonoBehaviour {
    public GameObject menu;
    public GameObject stambar;
    public GameObject healthbar;
    
    public GameObject drop;

    public GameObject buildcanvas;
    public GameObject thirdPerson;
    public GameObject build;
    bool thirdPersonActive = true;
    bool buildActive = false;
    void Start()
    {
        // menu.SetActive(false);
    }
	public void Update () {
        if (Input.GetKey("escape")){
            Application.Quit();}
            
        /* if (drop.activeSelf){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } */ // TODO: Broken
	}


    public void TaskOnClick(){
        menu.SetActive(false);
        buildcanvas.SetActive(false);	
        stambar.SetActive(false);	
        healthbar.SetActive(false);
        drop.SetActive(true);
    }
    public void ExitOnClick(){
        Application.Quit();	
    }

     public void CamSwitchOnClick(){
        drop.SetActive(false);
        thirdPersonActive = !thirdPersonActive;
        buildActive = !buildActive;
        thirdPerson.SetActive(thirdPersonActive);
        build.SetActive(buildActive);
        menu.SetActive(true);
     }

    public void ResumeOnClick(){
        menu.SetActive(false);
        buildcanvas.SetActive(true);
        stambar.SetActive(true);	
        healthbar.SetActive(true);		

    }
}
