using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	
		//Focus point of camera
		public GameObject defaultFocus;
		private Camera camera;
		public float distance;
		public float angle;
		public float height;

		//zoom clamping
		public float minDistance;
		public float maxDistance;
		public float increment;
	
		//cached objects
		private Transform thisTransform;
		public Transform parentTransform;
	
		void Start ()
		{
				thisTransform = transform;
				parentTransform = transform.parent;
				camera = Camera.main;
				this.transform.parent.transform.position = defaultFocus.transform.position;
				camera.fieldOfView = 5;
				moveCamera ();
		}
	
		private void followPlayer ()
		{
				this.transform.parent.transform.position = defaultFocus.transform.position;
		}

		private void moveCamera ()
		{
				var xp = distance * Mathf.Cos (angle);
				var zp = distance * Mathf.Sin (angle);
				Vector3 newPosition = new Vector3 (xp, height, zp);
				transform.localPosition = newPosition;
				//Make sure the camera is still looking at the player.
				transform.LookAt (defaultFocus.transform.position);
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
				this.transform.parent.transform.position = defaultFocus.transform.position;

				//perform zoom -orthograpic only
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
		}
	
		public void changeFocus (GameObject o)
		{
				this.defaultFocus = o;
		
		}
}
