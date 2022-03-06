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
    private void OnMouseDown()
    {
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
        transform.position = GetMouseWorldPos() + mOffset;
        var position = new Vector3(Mathf.RoundToInt(this.transform.position.x / this.gridSize.x) * this.gridSize.x, (Mathf.RoundToInt(this.transform.position.y / this.gridSize.y) * this.gridSize.y), (Mathf.RoundToInt(this.transform.position.z / this.gridSize.z) * this.gridSize.z));
        this.transform.position = position;
        if(Input.GetKey(KeyCode.R))
        {

        }
    }
}
