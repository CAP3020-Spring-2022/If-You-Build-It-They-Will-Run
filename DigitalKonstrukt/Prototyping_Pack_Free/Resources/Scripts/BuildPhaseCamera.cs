using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPhaseCamera : MonoBehaviour
{
    public float mouseSensitivity = 10;
    float yaw;
    float pitch;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        Vector3 targetRotation = new Vector3(pitch, yaw);
        transform.eulerAngles = targetRotation;
    }
}
////////////////////////////HEY THIS IS JUST ELEMENTARY. ITS BASED OFF OF AN UNFINISHED VERSION OF THE THIRD PERSON CAMERA BUT I THOUGHT ITD BE A USEFUL START FOR WHEN WE GET HERE