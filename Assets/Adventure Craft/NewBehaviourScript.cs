using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour
{

		public float speed = 6.0F;
		public float jumpSpeed = 8.0F;
		public float gravity = 20.0F;
		private Vector3 moveDirection = Vector3.zero;
		
		void Update ()
		{
				CharacterController controller = GetComponent<CharacterController> ();
				Vector3 direction = Camera.main.transform.forward.normalized;
				if (controller.isGrounded) {
						if (Input.GetKey (KeyCode.W)) {
								moveDirection = direction; 
						} else if (Input.GetKey (KeyCode.S)) {
								moveDirection = -1f * direction; 
						} else {
								moveDirection = new Vector3 (0, 0, 0);
						}
						
						
						//moveDirection = transform.TransformDirection (moveDirection);
						moveDirection *= speed;
						if (Input.GetButton ("Jump"))
								moveDirection.y = jumpSpeed;
			
				}
				moveDirection.y -= gravity * Time.deltaTime;
				controller.Move (moveDirection * Time.deltaTime);
		}
}
