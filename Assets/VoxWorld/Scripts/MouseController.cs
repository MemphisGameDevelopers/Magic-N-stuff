using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour
{


		public GameObject leftClickAction;
		public GameObject rightClickAction;
		public int height; //TODO find a better place to handle this.
		private EffectFactory effectFactory;
		

		public void Start ()
		{
				effectFactory = new EffectFactory ();
		effectFactory.height = this.height;
		}

		public void Update ()
		{

				if (Input.GetButtonDown ("Fire1")) {
						//terrain.ReplaceBlockCursor(0);
					 	effectFactory.createProjectileTowardsMouse (leftClickAction);
				} else if (Input.GetButtonDown("Fire2")) {
						//terrain.AddBlockCursor(1);
						effectFactory.createEffectAtMousePositionGround (rightClickAction);
				}
		}
}


