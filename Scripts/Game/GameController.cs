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
    bool buildCanvasActive = false;

    public GameObject buildcanvas;
    void Start()
    {
        buildcanvas.SetActive(false);
        // menu.SetActive(false);
    }
	public void Update () {
        if (Input.GetKey("escape")){
            Application.Quit();}
            
       if (drop.activeSelf){
            Cursor.lockState = CursorLockMode.None;
           Cursor.visible = true;
            }
	}


    public void TaskOnClick(){
        menu.SetActive(true);
       // buildcanvas.SetActive(false);	
      //  stambar.SetActive(false);	
       // healthbar.SetActive(false);	

    }
    public void ExitOnClick(){
        Application.Quit();	
    }

     public void camOnClick(){
        print("Cam");
        buildCanvasActive = !buildCanvasActive;
        buildcanvas.SetActive(buildCanvasActive); 
        stambar.SetActive(true);
        healthbar.SetActive(true);
    }

    public void ResumeOnClick(){
        print("Clicked");
        // menu.SetActive(false);
        drop.SetActive(false);
        buildcanvas.SetActive(buildCanvasActive);
        stambar.SetActive(true);	
        healthbar.SetActive(true);		

    }
}
