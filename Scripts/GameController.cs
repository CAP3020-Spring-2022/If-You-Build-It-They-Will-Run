// A8 also used in all previous assignments
// GameController.cs by Erin Hu
// Description: Escapes the program

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public void Update () {
        if (Input.GetKey("escape"))
            Application.Quit();
	}
}
