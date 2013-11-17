using UnityEngine;
using System.Collections;

/** Small example script of using the LocalAvoidance class */
[RequireComponent(typeof(LocalAvoidance))]
public class LocalAvoidanceMover : MonoBehaviour {
	
	public float targetPointDist = 10;
	public float speed = 2;
	
	Vector3 targetPoint;
	LocalAvoidance controller;
		
	// Use this for initialization
	void Start () {
		targetPoint = transform.forward*targetPointDist + transform.position;
		controller = GetComponent<LocalAvoidance>();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (controller != null) {
			//The LocalAvoidance controller can be called just like a CharacterController
			controller.SimpleMove ((targetPoint-transform.position).normalized*speed);
		}
		/* else {
			GetComponent<CharacterController>().SimpleMove ((targetPoint-transform.position).normalized*speed);
		}*/
	}
}
