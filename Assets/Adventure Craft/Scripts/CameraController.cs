using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	
		//Focus point of camera
		private GameObject player;
		private Camera camera;
		public float distance;
		public float zoom;
		public float elevationAngle;
		public float radialAngle;
		public float height;

		//zoom clamping
		public float minDistance;
		public float maxDistance;
		public float increment;

		//cached objects
		private Transform thisTransform;
		private Transform pivot;
		private Hashtable ht;
		
		void Start ()
		{
				player = GameObject.FindGameObjectWithTag ("Player");
				thisTransform = transform;
				pivot = transform.parent;
				camera = Camera.main;
				
				elevationAngle = Mathf.Atan (height / distance);
				zoom = height / Mathf.Sin (elevationAngle);
				
				ht = new Hashtable ();
				ht.Add ("isLocal", true);
				ht.Add ("time", 1f);
				ht.Add ("easetype", iTween.EaseType.easeInOutSine);
				
				moveCamera ();
		}
	
		private void tweenCamera ()
		{
				var xp = distance * Mathf.Cos (radialAngle);
				var zp = distance * Mathf.Sin (radialAngle);
				
				Vector3 newPosition = new Vector3 (xp, height, zp);
				//transform.localPosition = newPosition;
				ht.Remove ("position");
				ht.Add ("position", newPosition);
				iTween.MoveTo (pivot.gameObject, ht);
				iTween.LookUpdate (camera.gameObject, player.transform.position, .5f);
				//Make sure the camera is still looking at the player.
				//transform.LookAt (player.transform.position);
		}
		
		private void moveCamera ()
		{
				var xp = distance * Mathf.Cos (radialAngle);
				var zp = distance * Mathf.Sin (radialAngle);
		
				Vector3 newPosition = new Vector3 (xp, height, zp);
				pivot.position = newPosition;
				//Make sure the camera is still looking at the player.
				transform.LookAt (player.transform.position);
		}
	
	
		public void spinCamera (float dif)
		{
				float radians = dif * Mathf.Deg2Rad;
				radialAngle += radians;
				//tweenCamera ();

		}

		private void zoomCamera (float change)
		{
				if (zoom + change >= minDistance && zoom + change <= maxDistance) {
						zoom += change;
						height = zoom * Mathf.Sin (elevationAngle);
						distance = zoom * Mathf.Sin (elevationAngle);
						//tweenCamera ();
				}
		}
		void Update ()
		{
				pivot.parent.transform.position = player.transform.position;
				
				if (Input.GetAxis ("Mouse ScrollWheel") < 0) { // back
						zoomCamera (-1f * increment);
						tweenCamera ();
				} else if (Input.GetAxis ("Mouse ScrollWheel") > 0) { // forward
						zoomCamera (increment);
						tweenCamera ();
				} else if (Input.GetKeyDown ("q")) {
						spinCamera (45f);
						tweenCamera ();
				} else if (Input.GetKeyDown ("e")) {
						spinCamera (-45f);
						tweenCamera ();
				}
				
				transform.LookAt (player.transform.position);

		}
	
		public void changeFocus (GameObject o)
		{
				this.player = o;
		
		}
}
