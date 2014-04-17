#pragma strict

var buttons : GUITexture[];
var radius : float = 100;
var verticalStretch : float = 1.5;
var offset : Vector2;
var fullCircle : float = 360;

private var distanceCurve : AnimationCurve;
private var sizeCurve : AnimationCurve;

private var disableOtherClicks : boolean = false;

private var disableFade : float = 0.0;

var animationComponent : Animation;
var startAnimation : String;

var staff : Transform;
var dagger : Transform;

private var cursorAngle : float;

var viewModel : ViewModel;

private var animationName : String;

private var holsterCurve : AnimationCurve;

var platformTransform : Transform;

function Start () {
	distanceCurve = new AnimationCurve(Keyframe(0,.9), Keyframe(80,.5),Keyframe(300,.2),Keyframe(800,0));
	sizeCurve = new AnimationCurve(Keyframe(0,64), Keyframe(300,48), Keyframe (800,32),Keyframe(800,16));
	animationComponent.Blend(startAnimation);
	
	holsterCurve = new AnimationCurve(Keyframe(0,1),Keyframe(0.2,1), Keyframe(0.5,0), Keyframe(1,0));
}

function Update () {
	cursorAngle = Mathf.Rad2Deg * 
	Mathf.Atan2(Input.mousePosition.x - Screen.width*.5 - offset.x, Input.mousePosition.y - Screen.height *.5 - offset.y) - 90;
	
	//Fade.
	if(!viewModel.CanIClick()){
		disableFade += Time.deltaTime;
	}
	else{
		disableFade -= Time.deltaTime;
	}
	disableFade = Mathf.Clamp01(disableFade);
	
	var clickLock : boolean = false;
	if(Input.GetMouseButtonUp(0)){
		disableOtherClicks = false;
	}
	for (var i = 0; i < buttons.Length; i++){
		var buttonAngle : float = -i * (fullCircle / (buttons.Length-1 ));

		buttons[i].pixelInset.x = (Mathf.Cos(Mathf.Deg2Rad * (-i * (fullCircle / (buttons.Length-1 )))) * radius)  + offset.x;
		buttons[i].pixelInset.y = (Mathf.Sin(Mathf.Deg2Rad * (-i * (fullCircle / (buttons.Length-1)))) * radius * verticalStretch) + offset.y;

		//Distance to cursor.
		var cursorDistance : float = Vector2.Distance(Vector2(Input.mousePosition.x ,Input.mousePosition.y),
		Vector2(
		(Screen.width * buttons[i].transform.position.x + buttons[i].pixelInset.x) + buttons[i].pixelInset.width*.5, 
		(Screen.height * buttons[i].transform.position.y + buttons[i].pixelInset.y) + buttons[i].pixelInset.height*.5));
		
		//Size.		
		buttons[i].pixelInset.width = sizeCurve.Evaluate(cursorDistance);
		buttons[i].pixelInset.height = sizeCurve.Evaluate(cursorDistance);
		
		//Check Button.
		if(!clickLock && viewModel.CanIClick() && Input.GetMouseButton(0) && buttons[i].HitTest(Input.mousePosition)){
			buttons[i].pixelInset.width *= .8;
			buttons[i].pixelInset.height *= .8;
			clickLock = true;
			
			disableOtherClicks =true;
			
			animationName = buttons[i].GetComponent(StringHold).setString;
			
			animationComponent.CrossFade(animationName);
			
			switch(animationName){
				case "Dagger Stance":
				case "Dagger Strike 1":
				case "Dagger Strike 2":
				case "Dagger Strike 3":
				case "Dagger Strike 4":
				case "Draw Dagger":
					dagger.renderer.enabled = true;
					staff.renderer.enabled = false;
					break;
				case "Staff Stance":
				case "Staff Spell Circle":
				case "Staff Spell Forward":
				case "Staff Spell Ground":
				case "Staff Spell Upwards":
				case "Staff Swing":
				case "Draw Staff":
					dagger.renderer.enabled = false;
					staff.renderer.enabled = true;				
				break;
				default:
					dagger.renderer.enabled = false;
					staff.renderer.enabled = false;					
					break;
			}
			
		}		
		
		//Alpha.
		var alpha : float = distanceCurve.Evaluate(cursorDistance);
		buttons[i].color = Color(Mathf.Pow(alpha,1.1),Mathf.Pow(alpha,1.1),Mathf.Pow(alpha,0.95),alpha * (1 - disableFade));
	}
	
	var staffHolsterItem : HolsterItem = staff.GetComponent(HolsterItem);
	var daggerHolsterItem : HolsterItem = dagger.GetComponent(HolsterItem);	
	
	switch (animationName){
		case "Draw Dagger":
			daggerHolsterItem.transition = holsterCurve.Evaluate(
			animationComponent["Draw Dagger"].normalizedTime - Mathf.Floor(animationComponent["Draw Dagger"].normalizedTime) );
			break;
		case "Draw Staff":
			staffHolsterItem.transition = holsterCurve.Evaluate(
			animationComponent["Draw Staff"].normalizedTime - Mathf.Floor(animationComponent["Draw Staff"].normalizedTime) );
			break;
		case "Spin Left":
		case "Crouch Spin Left":
			platformTransform.Rotate(Vector3(0,0,90) * Time.deltaTime);
			break;
		case "Spin Right":
		case "Crouch Spin Right":
			platformTransform.Rotate(Vector3(0,0,-90) * Time.deltaTime);
			break;
		default:
			daggerHolsterItem.transition = 0.0;
			staffHolsterItem.transition = 0.0;	
		}	
}

function CanIClick() : boolean{
	return !disableOtherClicks;
}