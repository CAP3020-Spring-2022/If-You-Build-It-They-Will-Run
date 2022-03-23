using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 10;
    public Transform target;
    public float dstFromTarget = 5;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float yaw;
    float pitch;
    // Start is called before the first frame update
    void Start()
    {
        // Not for alpha, mouse is needed
        /* Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; */
    }

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        if(Input.mouseScrollDelta.y > 0.0f)
            dstFromTarget++;
        else if(Input.mouseScrollDelta.y < 0.0f)
            dstFromTarget--;

        Mathf.Clamp(dstFromTarget, 5.0f, 10.0f);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        transform.position = target.position - transform.forward * dstFromTarget;  
    }
}
