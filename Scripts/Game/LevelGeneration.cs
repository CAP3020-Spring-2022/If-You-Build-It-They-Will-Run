using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
	public GameObject[] objects;
	List<float> positionsX = new List<float>();
	List<float> positionsY = new List<float>();
	List<float> positionsZ = new List<float>();
	public float[,] bluenoise;
	int objectCount = 0;
	int objectMax = 0;
	float platformHeight = -350f;
	float endHeightModifier = 0;

    void Start()
    {
		objectMax = objects.Length;
		
		while ( (endHeightModifier < 0.6f && endHeightModifier >= 0f) || (endHeightModifier > -0.6 && endHeightModifier < 0))
			endHeightModifier = Random.Range(-1f,1f);
		//print(endHeightModifier);
		generatePositions();
		
		for (int i = 0; i < objectMax; i++) {
			//Instantiate(objects[i], new Vector3(positionsX[i], positionsY[i], positionsZ[i]), Quaternion.Euler(-90, 90, 90));
			if(objects[i].name == "FloatingIslandFlatTop")
				Instantiate(objects[i], new Vector3(positionsX[i], platformHeight - 2f, positionsZ[i]), Quaternion.Euler(0, 0, 0));
			else if(objects[i].name == "L-crate" || objects[i].name == "Staircase-Crate" || objects[i].name == "Big-L-Crate")
				Instantiate(objects[i], new Vector3(positionsX[i], platformHeight - 5f, positionsZ[i]), Quaternion.Euler(-90, 0, 90));
			else if(objects[i].name == "Skull-Pillar")
				Instantiate(objects[i], new Vector3(positionsX[i], platformHeight - 50f, positionsZ[i]), Quaternion.Euler(-90, 0, 90));
			else if(objects[i].name == "Wall-Run")
				Instantiate(objects[i], new Vector3(positionsX[i], platformHeight, positionsZ[i]), Quaternion.Euler(0, 90, 0));
			else if(objects[i].name == "Slide-Obstacle")
				Instantiate(objects[i], new Vector3(positionsX[i], platformHeight - 5f, positionsZ[i]), Quaternion.Euler(0, 90, 0));
			else if(objects[i].name == "Slide-Island")
				Instantiate(objects[i], new Vector3(positionsX[i], platformHeight - 2f, positionsZ[i]), Quaternion.Euler(0, 90, 0));
			else if(objects[i].name == "Ramp-Island")
				Instantiate(objects[i], new Vector3(positionsX[i], platformHeight - 2f, positionsZ[i]), Quaternion.Euler(-90, -90, 0));
			else if(objects[i].name == "Long-Island")
				Instantiate(objects[i], new Vector3(positionsX[i], platformHeight - 2f, positionsZ[i]), Quaternion.Euler(-90, -90, 0));
		}
    }
	
	void generatePositions() {
		int width = 60;
		int height = 60;
		int R = 7;
		
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
			  positionsX.Add(xc-((float)width/2f));
			  positionsY.Add(platformHeight*2 + (platformHeight * xc / (float)width * endHeightModifier * 1.1f) + (Random.Range(-platformHeight, platformHeight) * 0.1f));
			  positionsZ.Add(yc);
			  //print("Object " + objectCount + ": placing platform at " + positionsX[objectCount] + ", " + positionsY[objectCount] + ", " + positionsZ[objectCount] + "\n");
			  objectCount++;
			}
		  }
		}
	}
}
