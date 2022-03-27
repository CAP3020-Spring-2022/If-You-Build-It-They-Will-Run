using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCollider : MonoBehaviour
{
    bool collide = false;
    GetComponent <ParticleSystem>().Play ();
    ParticleSystem.EmissionModule em = GetComponent<ParticleSystem>().emission;
    em.enabled = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            collide = true;
            Debug.Log("trigger is set to true");
<<<<<<< Updated upstream
=======
            em.enabled = true;
>>>>>>> Stashed changes
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.name == "Player")
        {
            collide = false;
            em.enabled = false;
        }
    }
}
