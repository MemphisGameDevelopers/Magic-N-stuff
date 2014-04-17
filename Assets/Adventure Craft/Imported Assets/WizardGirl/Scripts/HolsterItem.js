#pragma strict

var holsterTransform : Transform;
var holsterLocalPosition : Vector3;
var holsterLocalRotation : Vector3;

var handTransform : Transform;
var handLocalPosition : Vector3;
var handLocalRotation : Vector3;

var transition : float; //1 is holster, 0 is hand.

function Start () {
	transform.parent = handTransform;
}

function LateUpdate () {
	var worldHolsterPosition : Vector3 = holsterTransform.TransformPoint(holsterLocalPosition);
	var worldHandPosition : Vector3 = handTransform.TransformPoint(handLocalPosition);
	transition = Mathf.Clamp01(transition);
	transform.position = Vector3.Lerp(worldHandPosition, worldHolsterPosition, transition);
	
	transform.parent = holsterTransform;
	transform.localRotation.eulerAngles = holsterLocalRotation;
	var worldHolsterRotation : Quaternion = transform.rotation;
	transform.parent = handTransform;
	transform.localRotation.eulerAngles = handLocalRotation;
	var worldHandRotation : Quaternion = transform.rotation;;
	transform.rotation = Quaternion.Slerp(worldHandRotation, worldHolsterRotation, transition);
}