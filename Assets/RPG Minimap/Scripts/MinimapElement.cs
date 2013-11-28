using UnityEngine;
using System.Collections;

public class MinimapElement : MonoBehaviour {
	
	Camera minimapCamera;

	// Use this for initialization
	void Start () {
		//Sind the minimap camera
		minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera").camera;
	}
	
	// Update is called once per frame
	void Update () {
		//Scale the element so that it allways stays the same size based on the size of the minimap
		transform.localScale = new Vector3(minimapCamera.orthographicSize / 5, minimapCamera.orthographicSize / 10, minimapCamera.orthographicSize / 5);
	}
}
