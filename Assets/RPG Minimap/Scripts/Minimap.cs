using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {
	
	Camera cam;								//The camera component of the minimap
	
	public Transform target;				//The target the minimap should follow
	
	public Texture2D minimapFrame;			//The frame texture around the minimap
	public Texture2D plusButton;			//The plus button texture used to zoom out
	public Texture2D minusButton;			//The minus button texture used to zoom in
	
	public Transform depthPlane;			//The plane that makes sure the minimap is draw an a circle
	public Transform playerCircle;			//The plane that holds the texture we want in the middle of the minimap
	
	public int minSize;						//How far can the player zoom in?
	public int maxSize;						//How far can the player zoom out?

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();	
		//Set the size of the camera between middle and max
		cam.orthographicSize = (minSize + maxSize) / 2;
	}
	
	// Update is called once per frame
	void Update () {
		
		//Follow the target
		transform.position = new Vector3(target.position.x , 700, target.position.z);
		
		//Calculate the size and position of the minimap
		cam.pixelRect = new Rect(Screen.width * 0.825f - Screen.height * 0.015f, Screen.height - Screen.width * 0.175f - Screen.height * 0.015f ,Screen.width * 0.175f,Screen.width * 0.175f);
		
		//Scale the depth plane so that the minimap is always as a perfect circle
		depthPlane.localScale = new Vector3(cam.orthographicSize, cam.orthographicSize, cam.orthographicSize);
		
		//Scale the circle in the middle of the minimap according to the size of the minimap
		playerCircle.localScale = new Vector3(cam.orthographicSize / 1.5f, cam.orthographicSize / 2.25f, cam.orthographicSize / 1.5f);
	}
	
	void OnGUI() {
		Event curEvent = Event.current;
		
		//Calculate the size and positon of the frame around the minimap
		Rect frameRect = new Rect(cam.pixelRect.x, Screen.height - cam.pixelRect.y - cam.pixelRect.height, cam.pixelRect.width, cam.pixelRect.height);
		GUI.DrawTexture(frameRect, minimapFrame);
		GUILayout.BeginArea(frameRect);
		//Calculate the size and positon of the frame around the plus button
		Rect plus = new Rect(frameRect.width * 0.825f, frameRect.height * 0.65f, frameRect.width * 0.15f, frameRect.width * 0.15f);
		//Calculate the size and positon of the frame around the minus button
		Rect minus = new Rect(frameRect.width * 0.675f, frameRect.height * 0.825f, frameRect.width * 0.15f, frameRect.width * 0.15f);
		GUI.DrawTexture(plus, plusButton);
		GUI.DrawTexture(minus, minusButton);
		
		//If we press the minus button zoom out
		if(plus.Contains(curEvent.mousePosition) && curEvent.type == EventType.MouseUp && curEvent.button == 0 && cam.orthographicSize > minSize) {
			cam.orthographicSize -= 2;
		}
		//If we press the plus button zoom in
		else if(minus.Contains(curEvent.mousePosition) && curEvent.type == EventType.MouseUp && curEvent.button == 0 && cam.orthographicSize < maxSize) {
			cam.orthographicSize += 2;
		}
		GUILayout.EndArea();
	}
}
