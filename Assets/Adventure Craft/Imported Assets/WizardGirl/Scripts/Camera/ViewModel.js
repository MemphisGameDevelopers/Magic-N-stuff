#pragma strict

var center : Vector3;
var centerTransform : Transform;

var radius : float = 3.0;

private var angle : float = 60;
private var angleSpeed : float;

var maxHeight : float = 3.0;
private var heightCapCurve : AnimationCurve;
var minHeight: float = 0.5;
private var lowerHeightCapCurve : AnimationCurve;
private var capRate : float;

private var moveVector : Vector3;

private var disableOtherClicks : boolean;

var buttonCircleAnimations : ButtonCircleAnimations;

function Start () {
	capRate = (maxHeight + minHeight) * .5;
	heightCapCurve = new AnimationCurve(Keyframe(minHeight,1), Keyframe(maxHeight - capRate, 1), Keyframe(maxHeight, 0));
	lowerHeightCapCurve = new AnimationCurve(Keyframe(minHeight,0), Keyframe(minHeight + capRate, 1), Keyframe(maxHeight, 1));
}

function Update () {
	//Center transform.
	if(centerTransform != null){
		center = Vector3.Lerp(center, centerTransform.position, Time.deltaTime 
		* Vector3.Distance(center, centerTransform.position) * .4);
	}
	
	var evaluateHeightCap : float = heightCapCurve.Evaluate(transform.position.y);
	//Input.
	if(Input.GetMouseButton(0) && buttonCircleAnimations.CanIClick()){
		disableOtherClicks = true;
		//Height.
		if(Input.GetAxis("Mouse Y") < 0){
			moveVector.y -= Input.GetAxis("Mouse Y")  * evaluateHeightCap;	
		}
		else{
			moveVector.y -= Input.GetAxis("Mouse Y") * lowerHeightCapCurve.Evaluate(transform.position.y);
		}
		//Angle.
		angleSpeed -= Mathf.Abs(Mathf.Pow(Input.GetAxis("Mouse X"),2.0)) * Mathf.Sign(Input.GetAxis("Mouse X")) * 20.0;
	}
	if(Input.GetMouseButtonUp(0)){
		disableOtherClicks = false;
	}
	//Horizontal position.
	angleSpeed = Mathf.Lerp(angleSpeed, 0, Time.deltaTime * 5.0);
	angle += angleSpeed * Time.deltaTime;
	
	var horizontalCenterDistance : float = Vector2.Distance(Vector2(transform.position.x, transform.position.z),
	Vector2(center.x,center.z));
	
	var desiredRadius : float = Mathf.Lerp(radius*.2, radius, evaluateHeightCap);
		
	var desiredHorizontalPosition : Vector2;
	desiredHorizontalPosition.x = Mathf.Cos(angle * Mathf.Deg2Rad) * desiredRadius;
	desiredHorizontalPosition.y = Mathf.Sin(angle * Mathf.Deg2Rad) * desiredRadius;
	
	transform.position.x = desiredHorizontalPosition.x;
	transform.position.z = desiredHorizontalPosition.y;
					
	//Slow down.
	moveVector = Vector3.Lerp(moveVector, Vector3.zero, Time.deltaTime * 5.0);
	
	if(moveVector.y > 0 && transform.position.y > maxHeight - capRate){
		moveVector.y = Mathf.Lerp(moveVector.y, 0, 1 - evaluateHeightCap);
	}
	
	//Apply movement.
	transform.Translate(moveVector * Time.deltaTime, Space.World);
	transform.position.y = Mathf.Clamp(transform.position.y, minHeight, maxHeight);
	
	//Look at center.
	transform.LookAt(center);
}

function CanIClick() : boolean{
	return !disableOtherClicks;
}