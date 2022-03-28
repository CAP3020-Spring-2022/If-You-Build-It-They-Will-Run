using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPhaseCamera : MonoBehaviour
{
    public float mouseSensitivity = 10;
    private float x;
    private float y;
    private Vector3 rotateValue;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, (float)0.05);
        }
        else if(Input.GetKey(KeyCode.A))
        {
            transform.Translate((float)-0.05, 0, 0);

        }
        else if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, (float)-0.05);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.Translate((float)0.05, 0, 0);
        }
        else if(Input.GetMouseButton(1))
        {
            y = Input.GetAxis("Mouse X") * 3;
            x = Input.GetAxis("Mouse Y") * 3;
            rotateValue = new Vector3(x, (float)(y * -3), 0);
            transform.eulerAngles = transform.eulerAngles - rotateValue;
        }
    }

    void Awake() {
        if(this.gameObject.activeSelf) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
