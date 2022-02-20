using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main controller for third person camera. Camera controls relative to mouse movement.
// Cursor locked in place during gameplay (can be disabled with ESC)
public class CameraController : MonoBehaviour
{

    [SerializeField] Transform cam;
    [SerializeField] float sens;
    float headPitch = 0.0f;
    float headPitchUpperLimit = 45.0f;
    float headPitchLowerLimit = -45.0f;
    

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; // invis
        Cursor.lockState = CursorLockMode.Locked; // locked
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse movement
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime * -1f; // love y axis in 3d space :)

        transform.Rotate(0f, mouseX, 0f);
        headPitch += mouseY;
        headPitch = Mathf.Clamp(headPitch, headPitchLowerLimit, headPitchUpperLimit); // makes sure you can't do any wonky camera angles (looking behind 
                                                               // you/upside down)
        cam.localEulerAngles = new Vector3(headPitch, 0f, 0f);
    }
}
