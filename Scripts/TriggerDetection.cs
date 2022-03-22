// TriggerDetection.cs by Erin Hu
// Description: Detects the collision and plays "Ding" sound

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetection : MonoBehaviour
{
    // public AudioClip dingClip;
    // AudioSource audioSource;
	    public int levelToLoad;

    void OnTriggerEnter(Collider myCollider){
        if (myCollider.gameObject.name == "Player"){
            Debug.Log("You have reached the end of the platform");
            Application.LoadLevel(levelToLoad);
        }

        // audioSource = GetComponent<AudioSource>();
        // audioSource.PlayOneShot(dingClip);
    }
}
