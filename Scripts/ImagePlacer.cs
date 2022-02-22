using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePlacer : MonoBehaviour
{
    //list of prefabs
    [SerializeField]
    private List<GameObject> imagePrefabs;

    //list of actual gameobjects
    [SerializeField]
    private List<GameObject> images;

    //dimensions of images
    [SerializeField]
    private float width, height, distance;


    [ContextMenu("Start")]
    //delete previous list of images and load new ones
    private void makeBackground()
    {
        foreach (var i in images)
        {
            DestroyImmediate(i);
        }
        images = new List<GameObject>();


        int iprefab = 0;
        float angle = 0f;

        for (int i = 0; i < 8; i++)
        { 
            images.Add(Instantiate(imagePrefabs[iprefab], transform));

            //rotate image 45 degrees for each location
            images[i].transform.Rotate(new Vector3(0f, angle, 0f));

            angle += 45f;
            iprefab++;

            if (iprefab >= imagePrefabs.Count)
                iprefab = 0;

        }

    }

    [ContextMenu("SetScale")]
    private void SetScale()
    {
        foreach(var i in images)
        {
            i.transform.localScale = new Vector3(width, height, 1f);
            float imageLength = images[0].GetComponent<SpriteRenderer>().bounds.size.x;
            distance = (imageLength / 2f + (Mathf.Sqrt(2) / 2) * imageLength) - 0.1f;
            i.transform.position = distance * i.transform.forward;
        }
    }

    private void OnValidate()
    {
        if(images.Count == 8)
        {
            SetScale();
        }
    }

}
