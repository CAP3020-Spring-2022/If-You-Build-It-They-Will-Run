using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPhaseCamera : MonoBehaviour
{
    public float mouseSensitivity = 10;
    private float x;
    private float y;
    private Vector3 rotateValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, (float)0.1);
        }
        else if(Input.GetKey(KeyCode.A))
        {
            transform.Translate((float)-0.1, 0, 0);

        }
        else if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, (float)-0.1);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.Translate((float)0.1, 0, 0);
        }
        else if(Input.GetMouseButton(1))
        {
            y = Input.GetAxis("Mouse X") * 3;
            x = Input.GetAxis("Mouse Y") * 3;
            rotateValue = new Vector3(x, y * -1, 0);
            transform.eulerAngles = transform.eulerAngles - rotateValue;
        }
       // yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
       // pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

       // Vector3 targetRotation = new Vector3(pitch, yaw);
       // transform.eulerAngles = targetRotation;
    }
}
////////////////////////////HEY THIS IS JUST ELEMENTARY. ITS BASED OFF OF AN UNFINISHED VERSION OF THE THIRD PERSON CAMERA BUT I THOUGHT ITD BE A USEFUL START FOR WHEN WE GET HERE