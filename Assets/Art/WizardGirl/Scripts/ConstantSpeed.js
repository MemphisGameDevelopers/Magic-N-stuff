#pragma strict

var vector : Vector3;
var local : boolean = true;

function Start () {

}

function Update () {
	if(local){
		transform.position += transform.TransformDirection(vector) * Time.deltaTime;
	}
	else{
		transform.position += vector * Time.deltaTime;
	}
}