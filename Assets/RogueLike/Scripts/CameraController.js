class CameraController extends MonoBehaviour{

	//Focus point of camera
	public var defaultFocus : GameObject;
	
	//Offset to show angle
	public var offsetX : float;
	public var offsetY : float;
	
	//zoom clamping
	public var minDistance : float;
	public var maxDistance : float;
	public var increment : float;
	
	//cached objects
	private var thisTransform : Transform;

	function Start(){
		thisTransform = transform;
	}
	function Update () {
	
		//Update the position to avatar + an offset
		thisTransform.position.x = defaultFocus.transform.position.x + offsetX;
		thisTransform.position.z = defaultFocus.transform.position.z + offsetY;
		
		//perform zoom -orthograpic only
		if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
		{
			camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - increment , minDistance);
			 
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
		{
			camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + increment, maxDistance);
		
		}
	}
	
	public function changeFocus(object : GameObject){
		this.defaultFocus = object;

	}
}
