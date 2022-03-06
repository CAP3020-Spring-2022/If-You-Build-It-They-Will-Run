using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelScript : MonoBehaviour {
//Attach this script to the GameManager and use it to load the other scene

	public int levelToLoad;

	public void ApplicationLoadLevel(){
		Application.LoadLevel (levelToLoad);
	}
}
