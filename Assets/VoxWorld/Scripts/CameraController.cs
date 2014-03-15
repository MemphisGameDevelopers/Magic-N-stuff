using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	
		//Focus point of camera
		private GameObject player;
		private Camera camera;
		public float distance;
		public float angle;
		public float height;

		//zoom clamping
		public float minDistance;
		public float maxDistance;
		public float increment;

		//camera spin
		private Vector3 mousePosMark;
		private Vector3 mousePosMark2;
		private bool isDragging = false;
		private float threshold = .5f;

		//cached objects
		private Transform thisTransform;
		private Transform parentTransform;
	
		void Start ()
		{
				player = GameObject.FindGameObjectWithTag ("Player");
				thisTransform = transform;
				parentTransform = transform.parent;
				camera = Camera.main;
				this.transform.parent.transform.position = player.transform.position;
				moveCamera ();
		}
	
		private void followPlayer ()
		{
				this.transform.parent.transform.position = player.transform.position;
		}

		private void moveCamera ()
		{
				var xp = distance * Mathf.Cos (angle);
				var zp = distance * Mathf.Sin (angle);
				Vector3 newPosition = new Vector3 (xp, height, zp);
				transform.localPosition = newPosition;
				//Make sure the camera is still looking at the player.
				transform.LookAt (player.transform.position);
		}
	
		public void spinCamera (float dif)
		{

				if (dif > 0) {
						angle += .05f;
				} else if (dif < 0) {
						angle -= .05f;
				}

				moveCamera ();

		}

		void Update ()
		{

				//base position stays on player
				this.transform.parent.transform.position = player.transform.position;

				if (Input.GetAxis ("Mouse ScrollWheel") < 0) { // back
						if (camera.orthographic) {
								camera.orthographicSize = Mathf.Max (Camera.main.orthographicSize - increment, minDistance);
						} else {
								camera.fieldOfView = Mathf.Max (camera.fieldOfView - increment, minDistance);
						}
				}
				if (Input.GetAxis ("Mouse ScrollWheel") > 0) { // forward
						if (camera.orthographic) {
								camera.orthographicSize = Mathf.Min (Camera.main.orthographicSize + increment, maxDistance);
						} else {
								camera.fieldOfView = Mathf.Min (camera.fieldOfView + increment, maxDistance);
						}
				}

				//Spin Camera on middle mouse drag
				if (Input.GetButtonDown ("Fire2")) {
						mousePosMark = Input.mousePosition;
				} else if (Input.GetButton ("Fire2")) {
						float dif = Input.mousePosition.x - mousePosMark.x; 
						if (Mathf.Abs (dif) > threshold) {
								isDragging = true;
								spinCamera (dif);
								mousePosMark = Input.mousePosition;
						}
				}

				//Reset camera
				if (Input.GetKeyDown ("z")) {
						angle = 10.21f;
						moveCamera ();
				}
		}
	
		public void changeFocus (GameObject o)
		{
				this.player = o;
		
		}
}
