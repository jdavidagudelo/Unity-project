#pragma strict
//MAPNAV Navigation ToolKit v.1.0

private var mapnav: MapNav;
private var gpsFix: boolean;
private var cam: Camera;

function Awake(){
	//Reference to the MapNav.js script and Main Camera
	mapnav=GameObject.FindGameObjectWithTag("GameController").GetComponent(MapNav);
	cam=Camera.main;
	//Set GUIText font size according to our device screen size
	guiText.fontSize=Mathf.Round(15*Screen.width/320);
}

function Start(){
	//Initialization message
	guiText.text= "Searching for satellites ...";
	//Set initial black screen (quad) height
	transform.parent.position.y=cam.transform.position.y+cam.camera.nearClipPlane;
	transform.parent.renderer.enabled=true;
	//Center this GUI Text
	transform.localPosition.x=0.5;
	transform.localPosition.z=(0.5-transform.parent.position.y);
}

function Update () {
	if(!mapnav.ready){
		//Display GPS fix and maps download progress
		guiText.text= mapnav.status;
	}
	else{
		//Clear messages once the map is ready
		guiText.text= "";	
		//Disable black screen quad
		transform.parent.renderer.enabled=false;
		//Disable this script (no longer needed)
		this.enabled=false;
	}
}
