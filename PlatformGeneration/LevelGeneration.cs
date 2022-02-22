using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
	
	public GameObject[] objects;
	List<float> positionsX = new List<float>();
	List<float> positionsY = new List<float>();
	public float[,] bluenoise;
	int objectCount = 0;
	int objectMax = 20;

    // Start is called before the first frame update
    void Start()
    {
		generatePositions();
		
		for (int i = 0; i < objectMax; i++) {
			Instantiate(objects[i], new Vector3(positionsX[i],15f,positionsY[i]), Quaternion.identity);
		}
    }
	
	void generatePositions() {
		int width = 200;
		int height = 200;
		int R = 6;
		
		bluenoise = new float[width,height];
		
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				float nx = (float)x/(float)width - 0.5f, ny = (float)y/(float)height - 0.5f;
				//bluenoise[y,x] = Mathf.PerlinNoise(50 * nx, 50 * ny);
				bluenoise[y,x] = Mathf.PerlinNoise(Random.Range(1, 50) * nx, Random.Range(1, 50) * ny);
				//print(bluenoise[y,x]);
			}
		}
		
		for (int yc = 0; yc < height; yc++) {
		  for (int xc = 0; xc < width; xc++) {
			float max = 0;
			for (int yn = yc - R; yn <= yc + R; yn++) {
			  for (int xn = xc - R; xn <= xc + R; xn++) {
				if (0 <= yn && yn < height && 0 <= xn && xn < width) {
				  float e = bluenoise[yn,xn];
				  if (e > max) { max = e; }
				}
			  }
			}
			if (objectCount < objectMax && bluenoise[yc,xc] == max) {
			  print("Object " + objectCount + ": placing platform at " + xc + ", " + yc + "\n");
			  positionsX.Add(xc-100f);
			  positionsY.Add(yc);
			  objectCount++;
			}
		  }
		}
		
		print("we are here.");
		
	}

}
