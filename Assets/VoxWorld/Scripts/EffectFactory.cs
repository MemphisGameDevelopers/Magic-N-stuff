using UnityEngine;
using System.Collections;

public class EffectFactory
{

		public float height;
		// Use this for initialization
		private Animator animator;
		private GameObject player;

		public EffectFactory ()
		{
		player = GameObject.FindGameObjectWithTag("Player");
		}
	
		public void createProjectileTowardsMouse (GameObject effect)
		{
				//animator.SetTrigger ("castTrigger");
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Vector3 eyeLevel = player.transform.position + (Vector3.up * height);
				Plane plane = new Plane ();
				plane.SetNormalAndPosition (Vector3.up, eyeLevel);
				float distance;
				if (plane.Raycast (ray, out distance)) {
						// some point of the plane was hit - get its coordinates
						Vector3 hitPoint = ray.GetPoint (distance);
						Vector3 relativePos = hitPoint - eyeLevel;
						Quaternion rotation = Quaternion.LookRotation (relativePos);
						GameObject.Instantiate (effect, eyeLevel, rotation);
			
				}
		}

		public void createEffectAtMousePositionAir (GameObject effect)
		{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Vector3 eyeLevel = player.transform.position + (Vector3.up * height);
				Plane plane = new Plane ();
				plane.SetNormalAndPosition (Vector3.up, eyeLevel);
				float distance;
				if (plane.Raycast (ray, out distance)) {
						// some point of the plane was hit - get its coordinates
						Vector3 hitPoint = ray.GetPoint (distance);
						GameObject o = GameObject.Instantiate (effect) as GameObject;
						o.transform.Translate (hitPoint);
			
				}
		}

		public void createEffectAtMousePositionGround (GameObject effect)
		{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Vector3 eyeLevel = player.transform.position;
				Plane plane = new Plane ();
				plane.SetNormalAndPosition (Vector3.up, eyeLevel);
				float distance;
				if (plane.Raycast (ray, out distance)) {
						// some point of the plane was hit - get its coordinates
						Vector3 hitPoint = ray.GetPoint (distance);
						GameObject o = GameObject.Instantiate (effect) as GameObject;
						o.transform.Translate (hitPoint);
			
				}
		}

		public void createEffectCasterGroundFollow (GameObject effect)
		{
				GameObject o = GameObject.Instantiate (effect) as GameObject;

		}

		public void createEffectCasterGroundStatic (GameObject effect)
		{
				GameObject o = GameObject.Instantiate (effect) as GameObject;
		
		}

}
