using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tutorials used:
// https://www.youtube.com/watch?v=VcWmGovo9ZA&ab_channel=SaeedPrez
// 
public class dragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    [SerializeField] private Vector3 gridSize = new Vector3(5, 5, 5);
    public GameObject cam1;
    public GameObject cam2;
    Vector3 mouse;
    int count = 0;
    // Used to tell if the object is in a movable state
    bool canMove = false;
    // Used to prevent object from snapping to mouse pos. when move key is released
    bool keyedMove = false;
    bool canRotate = true;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            canRotate = true;
            canMove = true;
        }
        if(Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.Z))
        {
            keyedMove = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            canMove = false;
        }
        if (Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.Y) || Input.GetKeyUp(KeyCode.Z))
        {
            canMove = false;
            keyedMove = false;
        }
    }
    private void OnMouseDown()
    {
        if(!cam1.active)
        {
            return;
        }
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        //store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }



    private Vector3 GetMouseWorldPos()
    {
        // pixel to world transform
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);

    }
    private void OnMouseDrag()
    {
        if (!cam1.active || !canMove)
        {
            return;
        }
        var position = new Vector3(0, 0, 0);
        if(Input.GetKey(KeyCode.X))
        {
            mouse = GetMouseWorldPos() + mOffset;
            position = new Vector3(Mathf.RoundToInt(mouse.x / this.gridSize.x) * this.gridSize.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
            this.transform.position = position;
        }
        else if(Input.GetKey(KeyCode.Y))
        {
            mouse = GetMouseWorldPos() + mOffset;
            position = new Vector3(this.gameObject.transform.position.x, Mathf.RoundToInt(mouse.y / this.gridSize.y) * this.gridSize.y, this.gameObject.transform.position.z);
            this.transform.position = position;
        }
        else if(Input.GetKey(KeyCode.Z))
        {
            mouse = GetMouseWorldPos() + mOffset;
            position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, Mathf.RoundToInt(mouse.z / this.gridSize.z) * this.gridSize.z);
            this.transform.position = position;
        }
        else if(Input.GetKey(KeyCode.R) && canRotate) {
            this.gameObject.transform.Rotate(0, 0, 90);
            canRotate = false;
        }
        else if(!keyedMove)
        {
                transform.position = GetMouseWorldPos() + mOffset;
                position = new Vector3(Mathf.RoundToInt(this.transform.position.x / this.gridSize.x) * this.gridSize.x, (Mathf.RoundToInt(this.transform.position.y / this.gridSize.y) * this.gridSize.y), (Mathf.RoundToInt(this.transform.position.z / this.gridSize.z) * this.gridSize.z));
                this.transform.position = position;

        }
    }
}
