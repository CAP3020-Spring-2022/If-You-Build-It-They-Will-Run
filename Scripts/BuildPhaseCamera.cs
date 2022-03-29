using UnityEngine;

public class BuildPhaseCamera : MonoBehaviour
{
    public float mouseSensitivity = 10;
    private float x;
    private float y;
    private Vector3 rotateValue;
    [SerializeField] float speed;
    // Update is called once per frame
    // Spacebar == Up
    // Control == Down
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            if(Input.GetKey(KeyCode.A))
            {
                transform.Translate(-speed / 2.0f, 0, speed / 2.0f);
            }
            else if(Input.GetKey(KeyCode.S))
            {
                transform.Translate(0, 0, 0);
            }
            else if(Input.GetKey(KeyCode.D))
            {
                transform.Translate(speed / 2.0f, 0, speed / 2.0f);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(0, (float)0.01, speed / 2.0f);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.Translate(0, -(float)0.01, speed / 2.0f);
            }
            else
            {
                transform.Translate(0, 0, speed);
            }
       
        }
        else if(Input.GetKey(KeyCode.A))
        {
            if(Input.GetKey(KeyCode.S))
            {
                transform.Translate(-speed / 2.0f, 0, -speed / 2.0f);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(-speed / 2.0f, (float)0.01, 0);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.Translate(-speed / 2.0f, -(float)0.01, 0);
            }
            else
            {
                transform.Translate(-speed, 0, 0);
            }
        

        }
        else if(Input.GetKey(KeyCode.S))
        {
            if(Input.GetKey(KeyCode.D))
            {
                transform.Translate(speed / 2.0f, 0, -speed / 2.0f);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(0, (float)0.01, -speed / 2.0f);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.Translate(0, -(float)0.01, -speed / 2.0f);
            }
            else
            {
                transform.Translate(0, 0, -speed);
            }
        }
        else if(Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(speed / 2.0f, (float)0.01, 0);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.Translate(speed / 2.0f, -(float)0.01, 0);
            }
            transform.Translate(speed, 0, 0);
        }
        else if(Input.GetMouseButton(1))
        {
            y = Input.GetAxis("Mouse X") * 3;
            x = Input.GetAxis("Mouse Y") * 3;
            rotateValue = new Vector3(x, (y * -3), 0);
            transform.eulerAngles = transform.eulerAngles - rotateValue;
        }
        else if(Input.GetKey(KeyCode.Space))
        {
            transform.Translate(0, (float)0.03, 0);
        }
        else if(Input.GetKey(KeyCode.LeftControl))
        {
            transform.Translate(0, -(float)0.03, 0);
        }
    }

    void Awake() {
        if(this.gameObject.activeSelf) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
