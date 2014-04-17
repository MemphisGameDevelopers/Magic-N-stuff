#pragma strict

@script RequireComponent(Light);

var cameraTransform : Transform;
var maxIntensity : float = 2.5;
var minIntensity : float = .8;

function Start () {

}

function Update () {
	light.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.Pow(Vector2.Angle(Vector2(transform.forward.x,transform.forward.z),
	Vector2(cameraTransform.forward.x, cameraTransform.forward.z)) / 180,2));
	
}