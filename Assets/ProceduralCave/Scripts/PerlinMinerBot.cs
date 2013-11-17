using UnityEngine;
using System.Collections;

public class PerlinMinerBot : MonoSingleton<PerlinMinerBot>{
	public GameObject prefabWall01; 
	public float scale;
	public override void Init()
	{
		int count = 0;
		for(float i = 0; i < 200; i++){
			for(float j = 0; j < 200; j++){
				float perlin1 =(Mathf.PerlinNoise(i/scale,26236));
				float perlin2 =(Mathf.PerlinNoise(j/scale,76));
				float perlin3 = (Mathf.PerlinNoise(count++/scale,156349));
				GameObject floor = GameObject.Instantiate(prefabWall01,new Vector3(i,perlin1*perlin2*perlin3*40,j),Quaternion.identity) as GameObject;
			}
		}
	}
	
	public void Update(){
		
	}
}
