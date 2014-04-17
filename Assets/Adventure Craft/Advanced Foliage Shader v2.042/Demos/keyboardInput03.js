// #pragma strict

function Start () {

}

function Update () {

	// forward
	if(Input.GetKeyDown("1")){
		Camera.main.renderingPath = RenderingPath.Forward;
	}
	// deferred
	if(Input.GetKeyDown("2")){
		Camera.main.renderingPath = RenderingPath.DeferredLighting;
	}
	
	if(Input.GetKeyDown("3")){
		var mainlight3 : GameObject;
		mainlight3 = GameObject.Find("01 Sun");
		mainlight3.transform.RotateAround (mainlight3.transform.position, Vector3.up, 10.0);
	}
}