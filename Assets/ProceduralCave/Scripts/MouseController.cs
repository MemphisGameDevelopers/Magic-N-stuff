using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour
{
		public CameraController cameraGO;
		//public ModifyTerrain terrain;
		public GameObject leftClickAction;
		public GameObject rightClickAction;
	public int height; //TODO find a better place to handle this.
		private EffectFactory effectFactory;
		//For testing if the user held down the mouse and dragged it.
		private Vector3 mousePosMark;
		private Vector3 mousePosMark2;
		private bool isDragging = false;
		private float mouseDownTime;
		private float threshold = .25f;

		public void Start ()
		{
				effectFactory = new EffectFactory ();
		effectFactory.height = this.height;
		}

		public void Update ()
		{


				//Spin Camera on middle mouse drag
				if (Input.GetButtonDown ("Fire3")) {
						mousePosMark = Input.mousePosition;
				} else if (Input.GetButton ("Fire3")) {
						float dif = Input.mousePosition.x - mousePosMark.x; 
						if (Mathf.Abs (dif) > 1) {
								isDragging = true;
								cameraGO.spinCamera (dif);
								mousePosMark = Input.mousePosition;
						}
				}

				if (Input.GetButtonDown ("Fire1")) {
						//terrain.ReplaceBlockCursor(0);
					 	effectFactory.createProjectileTowardsMouse (leftClickAction);
				} else if (Input.GetButtonDown("Fire2")) {
						//terrain.AddBlockCursor(1);
						effectFactory.createEffectAtMousePositionGround (rightClickAction);
				}
		}
}


