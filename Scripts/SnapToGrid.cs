using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Snap();
    }
    private void Snap()
    {
        var position = new Vector3(Mathf.RoundToInt(this.transform.position.x), (Mathf.RoundToInt(this.transform.position.y)), (Mathf.RoundToInt(this.transform.position.z)));
        this.transform.position = position;
    }
}
