#pragma strict

var rotationVector : Vector3;

function Start () {

}

function Update () {
	transform.rotation.eulerAngles += rotationVector * Time.deltaTime;
}