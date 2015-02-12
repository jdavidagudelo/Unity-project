#pragma strict
//MAPNAV Navigation ToolKit v.1.0
//Attention: This script uses a custom editor inspector: MAPNAV/Editor/MapNavInspector.cs

var user : Transform;							 //User(Player) transform
var simGPS : boolean=true;						 //True when the GPS Emulator is enabled
var userSpeed : float = 5.0;					 //User speed when using the GPS Emulator (keyboard input)
var realSpeed : boolean = false;				 //If true, the perceived player speed depends on zoom level(realistic behaviour)
var fixLat : float=42.3627;					     //Latitude
var fixLon : float=-71.05686;					 //Longitude
var altitude : float;							 //Current GPS altitude
var heading : float;							 //Last compass sensor reading (Emulator disabled) or user's eulerAngles.y (Emulator enabled)
var accuracy : float;							 //GPS location accuracy (error)
var maxZoom : int=18;							 //Maximum zoom level available. Set according to your maps provider
var minZoom : int=1;							 //Minimum zoom level available
var zoom : int=17;								 //Current zoom level
var multiplier:int; 							 //1 for a size=640x640 tile, 2 for size=1280*1280 tile, etc.
var key : String="Paste your Appkey here";       //AppKey (API key) code obtained from your maps provider (MapQuest, Google, etc.)
var maptype : String[];							 //Array including available maptypes
var index :int;									 //maptype array index. 
var camDist : float=15.0;						 //Camera distance(3D) or height(2D) to user
var camAngle : int=40;							 //Camera angle from horizontal plane
var initTime : int = 3;							 //Hold time after a successful GPS fix in order to improve location accuracy
var maxWait : int = 30;							 //GPS fix timeout
var buttons : boolean=true;						 //Enables GUI sample control buttons 
var dmsLat : String;							 //Latitude as degrees, minutes and seconds
var dmsLon : String;							 //Longitude as degrees, minutes and seconds
var updateRate : float = 0.1;					 //User's position update rate
var autoCenter : boolean=true;					 //Autocenter and refresh map
var fixPointer : boolean=true;					 //Fix user's localScale whatever the zoom level is (2D mode only)
var status : String;							 //GPS and other status messages
var gpsFix : boolean;							 //True after a successful GPS fix 
var iniRef : Vector3;							 //First location data retrieved on Start	 
var info : boolean;								 //Used by GPS-Status.js to enable/disable the GPS information window.
var triDView : boolean = false;					 //2D/3D modes toggle
var ready : boolean;							 //true when the map texture has been successfully loaded
var origin : String;
var destination : String;
private var speed : float;
private var cam : Transform;
private var mycam: Camera;
private var currentHeight : float;
private var loc : LocationInfo;
private var currentPosition : Vector3;
private var newUserPos : Vector3; 
private var currentUserPos : Vector3;
private var download : float;
private var www : WWW;
public var url = ""; 
private var longitude : double;
private var latitude : double;
private var rect : Rect;
private var mapping : boolean = false;
private var screenX : int;
private var screenY : int;
private var maprender : Renderer;
private var mymap : Transform;
private var initPointerSize :float;
private var tempLat :double;
private var tempLon :double;
private var  androidPlugin :AndroidJavaObject;
private  var activityContext :AndroidJavaObject;
private var lastLocation :AndroidJavaObject;
var markers:String = "";//parametro usado en la URL para marcadores
var pointsParams : String = "";//parametro usado para rutas
private var markerObjects : ArrayList = ArrayList();//Lista de marcadores
public var markerPrefab:GameObject;//Prefab usado para marcar lugares
public var placesName: String = "bar";//nombre de lugares cercanos
public var placesRadius :float = 200.0;//Radio de los lugares a los que se puede acceder
public var useNativeGps :boolean = true;
function Awake(){
	//Set the map's tag to GameController
	transform.tag="GameController";
	
	//References to the Main Camera and Player. 
	//Please make sure your camera is tagged as "MainCamera" and your user visualization/character as "Player"
	cam=Camera.main.transform;
	mycam=Camera.main.camera;
	user= GameObject.FindGameObjectWithTag("Player").transform;
	
	//Store most used components and values into variables for faster access.
	mymap = transform;
	maprender= renderer;
	screenX=Screen.width;
	screenY=Screen.height;
	
	//Set the camera's field of view according to Screen size so map's visible area is maximized.
	if(screenY>screenX){
		mycam.fieldOfView=72.5;
	}
	else{
		mycam.fieldOfView=95-(28*(screenX*1.0/screenY*1.0));
	}
	//Add possible values to maptype array. Change if using a maps provider other than MapQuest Open Static Maps.
	maptype = ["map","sat","hyb"];
}
public function setContext()
{
	try{
	androidPlugin.Call("setContext", activityContext);
	}
	catch(e)
	{
		text.text = e.Message;
	}
}
public var text : UnityEngine.UI.Text;
function OnDestroy()
{
	
}
function updateLocation()
{
	#if !UNITY_EDITOR && UNITY_ANDROID
	if(androidPlugin != null)
	{
	try
	{
		var lastLocation:AndroidJavaObject = androidPlugin.Call.<AndroidJavaObject>("getLocation");
		latitude = lastLocation.Call.<double>("getLatitude");
		longitude = lastLocation.Call.<double>("getLongitude");
	}
	catch(e)
	{
		text.text = e.Message;
	}
		//altitude = lastLocation.Call.<float>("getAltitude");
		
	}
	#endif
}
function Start () {
	#if !UNITY_EDITOR && UNITY_ANDROID
		if (androidPlugin == null) {
			var activityClass :AndroidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			if(activityClass != null)
			{
				activityContext = activityClass.GetStatic.<AndroidJavaObject>("currentActivity");
			}
			var pluginClass :AndroidJavaClass = new AndroidJavaClass("udea.telesalud.artica.com.plugin.AndroidPlugin");
			if(pluginClass != null)
			{
				androidPlugin = pluginClass.CallStatic.<AndroidJavaObject>("instance");
				
				activityContext.Call("runOnUiThread", new AndroidJavaRunnable(setContext));
			}
		}
	#endif
	//Setting variables values on Start
	gpsFix=false;
	rect = Rect (screenX/10, screenY/10,8*screenX/10, 8*screenY/10);
	mymap.eulerAngles.y=180;
	initPointerSize=user.localScale.x;
	user.position=Vector3(0,user.position.y,0);
	//Rotate the camera on Start to avoid showing unwanted scene elements during initialization  (e.g.GUITexts)
	cam.eulerAngles.x=270;
	//The "ready" variable will be true when the map texture has been successfully loaded.
	ready=false; 
	
	if(triDView)
		//Disable fixed size pointer on 3d view mode
		fixPointer=false;
	
	//STARTING LOCATION SERVICES
    // First, check if user has location service enabled
    if (!Input.location.isEnabledByUser){
    	//This message prints to the Editor Console
    	print("Please enable location services and restart the App");
    	//You can use this "status" variable to show messages in your custom user interface (GUIText, etc.)
    	status="Please enable location services\n and restart the App";
		yield WaitForSeconds(4);
		Application.Quit();
		return;
    }

    // Start service before querying location
    Input.location.Start (3,3); 
    Input.compass.enabled=true;
	print("Initializing Location Services..");
	status="Initializing Location Services..";

    // Wait until service initializes
    while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
        yield WaitForSeconds (1);
        maxWait--;
    }

    // Service didn't initialize in 30 seconds
    if (maxWait < 1) {
    	print("Unable to initialize location services.\nPlease check your location settings and restart the App");
		status="Unable to initialize location services.\nPlease check your location settings\n and restart the App";
		yield WaitForSeconds(4);
		Application.Quit();
        return;
    }

    // Connection has failed
    if ( Input.location.status == LocationServiceStatus.Failed) {
    	print("Unable to determine your location.\nPlease check your location setting and restart this App");
		status="Unable to determine your location.\nPlease check your location settings\n and restart this App";
		yield WaitForSeconds(4);
		Application.Quit();
        return;
    }
    
    // Access granted and location value could be retrieved
    if(maxWait >= 0) {
    	print("GPS Fix established. Setting position..");
		status="GPS Fix established!\n Setting position ...";
        if(!simGPS){
        	//Wait in order to find enough satellites and increase GPS accuracy
        	yield WaitForSeconds(initTime);
        	//Set position
        	if(useNativeGps)
        	{
        		updateLocation();
        	}
        	else
        	{
        		loc  = Input.location.lastData;
        		latitude = loc.latitude;
        		longitude = loc.longitude;
        	}          
        	iniRef.x = ((longitude * 20037508.34 / 180)/100);
   			iniRef.z = System.Math.Log(System.Math.Tan((90 + latitude) * System.Math.PI / 360)) / (System.Math.PI / 180);
  			iniRef.z = ((iniRef.z * 20037508.34 / 180)/100);  
  			iniRef.y = 0;
  			fixLon=longitude;
    		fixLat=latitude; 
    		//Successful GPS fix
    		gpsFix=true;
    		//Update Map for the current location
    		MapPosition();
  		}  
  		else{
  			//Simulate initialization time
  			yield WaitForSeconds(initTime);
  			//Set Position
  			iniRef.x = ((fixLon * 20037508.34 / 180)/100);
   			iniRef.z = System.Math.Log(System.Math.Tan((90 + fixLat) * System.Math.PI / 360)) / (System.Math.PI / 180);
  			iniRef.z = ((iniRef.z * 20037508.34 / 180)/100);  
  			iniRef.y = 0;
  			//Simulated successful GPS fix
  			gpsFix=true;
  			//Update Map for the current location
  			MapPosition();
  		}    
    }
    //Rescale map, set new camera height, and resize user pointer according to new zoom level
    ReScale();
}

//Set player's position using new location data (every "updateRate" seconds)
//Default value for updateRate is 0.1. Increase if necessary to improve performance
InvokeRepeating("MyPosition",1,updateRate); 

function MyPosition(){
	if(gpsFix){
		if(!simGPS){
			if(useNativeGps)
        	{
        		updateLocation();
        	}
        	else
        	{
        		loc  = Input.location.lastData;
        		latitude = loc.latitude;
        		longitude = loc.longitude;
        	} 
			newUserPos.x = ((longitude * 20037508.34 / 180)/100)-iniRef.x;
			newUserPos.z = System.Math.Log(System.Math.Tan((90 + latitude) * System.Math.PI / 360)) / (System.Math.PI / 180);
	    	newUserPos.z = ((newUserPos.z * 20037508.34 / 180)/100)-iniRef.z;  
	    	fixLon=longitude;
	    	fixLat=loc.latitude; 
		}
		else{
			newUserPos.x = ((fixLon * 20037508.34 / 180)/100)-iniRef.x;
			newUserPos.z = System.Math.Log(System.Math.Tan((90 + fixLat) * System.Math.PI / 360)) / (System.Math.PI / 180);
	    	newUserPos.z = ((newUserPos.z * 20037508.34 / 180)/100)-iniRef.z; 
	    	fixLon= (18000 * (user.position.x+iniRef.x))/20037508.34;
	    	fixLat= ((360/Mathf.PI)*Mathf.Atan(Mathf.Exp(0.00001567855943*(user.position.z+iniRef.z))))-90;	  
		}
		dmsLat=convertdmsLat(fixLat);
		dmsLon=convertdmsLon(fixLon);
	}	
} 

//Set player's orientation using new incoming compass data (every 0.05s)
InvokeRepeating("Orientate",1,0.05);
function Orientate(){
	if(!simGPS && gpsFix){
		heading=Input.compass.trueHeading;
	}
	else{
		heading=user.eulerAngles.y;
	}
}
 
//Get altitude and horizontal accuracy readings using new location data (every 2s)
InvokeRepeating("AccuracyAltitude",1,2);
function AccuracyAltitude(){
	if(gpsFix)
		altitude=loc.altitude;
		accuracy=loc.horizontalAccuracy;
}

//Auto-Center Map on 2D View Mode 
InvokeRepeating("Check",1,0.2);
function Check(){
	if(autoCenter && triDView==false){
		if(ready==true && mapping==false && gpsFix){
			if (rect.Contains(Vector2.Scale(mycam.WorldToViewportPoint (user.position),Vector2(screenX,screenY)))){
				//DoNothing
			}
			else{
				MapPosition();	
			}
		}
	}
}

//Auto-Center Map on 3D View Mode when exiting map's collider
function OnTriggerExit(other:Collider){
	if(other.tag=="Player" && autoCenter && triDView){
		MapPosition();
		ReScale();
	}
}

//Update Map with the corresponding map images for the current location
function MapPosition(){

	//The mapping variable will only be true while the map is being updated
	mapping=true;
	
	//CHECK GPS STATUS AND RESTART IF NEEDED
	
	if (Input.location.status == LocationServiceStatus.Stopped || Input.location.status == LocationServiceStatus.Failed){
   		// Start service before querying location
   		Input.location.Start (3,3);

    	// Wait until service initializes
   		var maxWait : int = 20;
   		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
   			yield WaitForSeconds (1);
   			maxWait--;
    	}

    	// Service didn't initialize in 20 seconds
    	if (maxWait < 1) {
        	print ("Timed out");
        	//use the status string variable to print messages to your own user interface (GUIText, etc.)
        	status="Timed out";
        	return;
    	}

    	// Connection has failed
    	if (Input.location.status == LocationServiceStatus.Failed) {
        	print ("Unable to determine device location");
        	//use the status string variable to print messages to your own user interface (GUIText, etc.)
        	status="Unable to determine device location";
        	return;
    	}
    
	}
	
   //------------------------------------------------------------------	//
   
	www=null;
	//Get last available location data
	if(useNativeGps)
    {
    	updateLocation();
    }
    else
    {
    	loc  = Input.location.lastData;
        latitude = loc.latitude;
        longitude = loc.longitude;
    } 
	//Make player invisible while updating map
	user.gameObject.renderer.enabled=false;
	
	//GPS simulator enabled
	if(simGPS){ 
		
		//Build a valid MapQuest OpenMaps tile request for the current location
		multiplier=2; //Since default tile size is 1280x1280 (640*multiplier). Modify as needed.
		
		//ATENTTION: If you want to implement maps from a different tiles provider, modify the following url accordingly to create a valid request
		//url="http://open.mapquestapi.com/staticmap/v4/getmap?key="+key+"&size=1280,1280&zoom="+zoom+"&type="+maptype[index]+"&center="+fixLat+","+fixLon;
		url = "http://maps.googleapis.com/maps/api/staticmap?key="+key+"&size=1280x1280&zoom="+zoom+"&center="+fixLat+","+fixLon;
		tempLat = fixLat; 
		tempLon = fixLon;

	}
	
	//GPS simulator disabled
	else{
		//Build a valid MapQuest OpenMaps tile request for the current location
		multiplier=2; 
		
		//ATENTTION: If you want to implement maps from a different tiles provider, modify the following url accordingly  to create a valid request
		//url="http://open.mapquestapi.com/staticmap/v4/getmap?key="+key+"&size=1280,1280&zoom="+zoom+"&type="+maptype[index]+"&center="+loc.latitude+","+loc.longitude;
		url = "http://maps.googleapis.com/maps/api/staticmap?key="+key+"&size=1280x1280&zoom="+zoom+"&center="+latitude+","+longitude;
		tempLat = latitude; 
		tempLon = longitude;
	}

	//Proceed with download if an Wireless internet connection is available 
	if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork){
		getRoute(origin, destination, "walking");
		
	}	
  	//Proceed with download if a 3G/4G internet connection is available 
	else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork){
		getRoute(origin, destination, "walking");
   		
	}
	//No internet connection is available. Switching to Offline mode.	 
	else{
		Offline();
	}	
}


/**
* Function utilizada para obtener la ruta entre las ubicaciones espcificadas como argumento. 
* Esta funcion utiliza el API de direcciones de Google para obtener la ruta para llegar desde la
* ubicacion origin hasta destination usando el medio de transporte especificado como argumento.
* origin: corresponde a un string que define la localizacion de origen del recorrido, esta localizacion
* puede especificarse como un par longitud, latitud o como direccion en texto no debe contener el simbolo &.
* destination: corresonde a un string que define la localizacion del destino del recorrido, esta localizacion
* puede especificarse como un par longitud, latitud o como direccion en texto. No debe contener el simbolo &.
* travelMode: corresponde al tipo decimal medio de transporte usado, puede ser driving, walking, bicycling o transit.
* Es posible que si se especifica un medio de transporte este no exista.
* Este almacena en la variable global pointsParams una lista de los puntos obtenidos en la ruta separados por el simbolo |,
* esta variable es utilizada como parte de la url usada para consumir mapas estaticos usando el api de google. Finalmente el 
* metodo invoca la funcion Online para actualizar el mapa.
*/
function getRoute(origin : String,  destination: String, travelMode :String){
	var urlLocal = "https://maps.googleapis.com/maps/api/directions/json?";
	urlLocal = urlLocal +"origin="+origin+"&destination="+destination+"&mode="+travelMode+"&sensor=false&key="+key;
	
	// Start a download of the given URL
	var wwwLocal = new WWW(urlLocal); 
	pointsParams = "";
	// Wait for download to complete
	download = (wwwLocal.progress);
	while(!wwwLocal.isDone){
		print("Updating map "+System.Math.Round(download*100)+" %");
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Updating map "+System.Math.Round(download*100)+" %";
		yield;
	}
	//Show download progress and apply texture
	if(wwwLocal.error==null){
		print("Updating map 100 %");
		print("Map Ready!");
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Updating map 100 %\nMap Ready!";
		yield WaitForSeconds (0.5);
		var tmp : Texture2D;
		tmp = new Texture2D(1280,1280,TextureFormat.RGB24,false);
		wwwLocal.LoadImageIntoTexture(tmp); 	
	}
	//Download Error. Switching to offline mode
	else{
		print("Map Error:"+wwwLocal.error);
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Map Error:"+wwwLocal.error;
		yield WaitForSeconds (1);
	}
	var result = JSON.Parse(wwwLocal.text);
	var routes = result["routes"];
	for(var route : JSONNode in routes)
	{
		var legs = route["legs"];
		for(var leg : JSONNode in legs)
		{
			var steps = leg["steps"];
			var count : int = steps.Count;
			var i :int =0;
			for(var step :JSONNode in steps)
			{
				var startLocation : JSONNode= step["start_location"];
				var endLocation : JSONNode = step["end_location"];
				pointsParams += startLocation["lat"]+","+startLocation["lng"]+"|";
				if(i >= count - 1)
				{
					pointsParams += endLocation["lat"]+","+endLocation["lng"]+"|";
				}
				i++;
			}
			break;
		}
		break;
	}
	if(pointsParams.length > 0)
	{
		pointsParams = pointsParams.Substring(0, pointsParams.length - 1);
	}
	getPlaces(placesName, origin, placesRadius);
}
function getPlaces(name : String,  location: String, radius :float){
	name=name.Replace(" ", "+");
	
	var urlPlaces:String = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?";
	urlPlaces = urlPlaces +"name="+name+"&location="+location+"&radius="+radius+"&key="+key;
	// Start a download of the given URL
	var wwwLocal = new WWW(urlPlaces); 
	markers = "";
	// Wait for download to complete
	download = (wwwLocal.progress);
	while(!wwwLocal.isDone){
		print("Updating map "+System.Math.Round(download*100)+" %");
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Updating map "+System.Math.Round(download*100)+" %";
		yield;
	}
	//Show download progress and apply texture
	if(wwwLocal.error==null){
		print("Updating map 100 %");
		print("Map Ready!");
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Updating map 100 %\nMap Ready!";
		yield WaitForSeconds (0.5);
		var tmp : Texture2D;
		tmp = new Texture2D(1280,1280,TextureFormat.RGB24,false);
		wwwLocal.LoadImageIntoTexture(tmp); 	
	}
	//Download Error. Switching to offline mode
	else{
		print("Map Error:"+wwwLocal.error);
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Map Error:"+wwwLocal.error;
		yield WaitForSeconds (1);
	}
	var complete = JSON.Parse(wwwLocal.text);
	var results = complete["results"];
	var i : int = 1;
	for(var marker:GameObject in markerObjects)
	{
		GameObject.Destroy(marker);
	}
	markerObjects.Clear();
	for(var result : JSONNode in results)
	{
		var latitude = result["geometry"]["location"]["lat"].Value;
		var longitude = result["geometry"]["location"]["lng"].Value;
		markers += "markers=color:blue%7Clabel="+i+"%7C"+latitude+","+longitude+"&";
		var marker = ObjectPool.instance.GetObjectForType(markerPrefab.name);//GameObject.Instantiate(markerPrefab, Vector3(0,0,0), Quaternion.identity);
		var setGeolocation : SetGeolocation = marker.GetComponent(SetGeolocation); 
		setGeolocation.lat = float.Parse(latitude);
		setGeolocation.lon = float.Parse(longitude);
		setGeolocation.GeoLocation();
		setGeolocation.SetTag(result["name"]);
		markerObjects.Add(marker);
		i++;
	}
	if(markers.length > 0)
	{
		markers = markers.Substring(0, markers.length - 1);
	}
	Online();
}
//Re-position map and camera using updated data
function ReSet(){
	transform.position.x = ((tempLon * 20037508.34 / 180)/100)-iniRef.x;
	transform.position.z = System.Math.Log(System.Math.Tan((90 + tempLat) * System.Math.PI / 360)) / (System.Math.PI / 180);
	transform.position.z = ((transform.position.z * 20037508.34 / 180)/100)-iniRef.z; 
	cam.position.x = ((tempLon * 20037508.34 / 180)/100)-iniRef.x;
	cam.position.z = System.Math.Log(System.Math.Tan((90 + tempLat) * System.Math.PI / 360)) / (System.Math.PI / 180);
	cam.position.z = ((cam.position.z * 20037508.34 / 180)/100)-iniRef.z; 
}


//ONLINE MAP DOWNLOAD
function Online(){	
	url = url+"&path=color:0xff0000ff|weight:3|"+pointsParams;//+"&"+markers;	
//	url = "https://www.google.com/maps/embed/v1/place?key="+key +"&q=Space+Needle,Seattle+WA";
	Debug.LogWarning(url);
	// Start a download of the given URL
	www = new WWW(url); 
	// Wait for download to complete
	download = (www.progress);
	while(!www.isDone){
		print("Updating map "+System.Math.Round(download*100)+" %");
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Updating map "+System.Math.Round(download*100)+" %";
		yield;
	}
	//Show download progress and apply texture
	if(www.error==null){
		print("Updating map 100 %");
		print("Map Ready!");
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Updating map 100 %\nMap Ready!";
		yield WaitForSeconds (0.5);
		maprender.material.mainTexture=null;
		var tmp : Texture2D;
		tmp = new Texture2D(1280,1280,TextureFormat.RGB24,false);
		maprender.material.mainTexture = tmp;
		www.LoadImageIntoTexture(tmp); 	
	}
	//Download Error. Switching to offline mode
	else{
		print("Map Error:"+www.error);
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Map Error:"+www.error;
		yield WaitForSeconds (1);
		maprender.material.mainTexture=null;
		Offline();
	}
	maprender.enabled=true;
	ReSet();
	user.gameObject.renderer.enabled=true;
	ready=true;
	mapping=false;
}

//USING OFFLINE BACKGROUND TEXTURE
function Offline(){
	maprender.material.mainTexture=Resources.Load("offline") as Texture2D;
	maprender.enabled=true;
	ReSet();
	ready=true;
	mapping=false;
	user.gameObject.renderer.enabled=true;
	
}


//Rescale map, set new camera height, and resize user pointer according to new zoom level
function ReScale(){
	while(mapping){
		yield;
	}
	mymap.localScale.x=multiplier*100532.244/(Mathf.Pow(2,zoom));
	mymap.localScale.z=transform.localScale.x;
	
	if(fixPointer){
		user.localScale.x=initPointerSize*65536/(Mathf.Pow(2,zoom));
		user.localScale.z=user.localScale.x;
	}
	
	//3D View 
	if(triDView){
			fixPointer=false;
			cam.localPosition.z=-(65536*camDist*Mathf.Cos(camAngle*Mathf.PI/180))/Mathf.Pow(2,zoom);
			cam.localPosition.y=65536*camDist*Mathf.Sin(camAngle*Mathf.PI/180)/Mathf.Pow(2,zoom);
	}
	//2D View 
	else{
		cam.localEulerAngles=Vector3(90,0,0);
		cam.position.y=(65536*camDist)/(Mathf.Pow(2,zoom));
		cam.position.z=user.position.z;
		//Correct the camera's near and far clipping distances according to its new height.
		//Introduced to avoid the player and plane not being rendered under some circunstances.
		mycam.nearClipPlane=cam.position.y/10;
		mycam.farClipPlane= cam.position.y+1;
		//Small correction to the user's height according to zoom level to avoid similar camera issues.
		user.position.y=10*Mathf.Exp(-zoom)+0.01;
	}
}

function Update(){
    //User pointer speed
    if(realSpeed){
		speed = userSpeed*0.05;
	}
	else{
		speed = speed=userSpeed*10000/(Mathf.Pow(2,zoom)*1.0);
	}	
	//3D-2D View Camera Toggle 
	if(triDView){
		cam.parent=user;
		if(ready)
			cam.LookAt(user);
	}
	else{
		cam.parent=null;		
	}	   
    if(ready){	
    	if(!simGPS){
    		//Smoothly move pointer to updated position and update rotation once the map has been successfully downloaded
    		currentUserPos.x = user.position.x;
			currentUserPos.x = Mathf.Lerp (user.position.x, newUserPos.x, 2.0 * Time.deltaTime);
			user.position.x = currentUserPos.x;
         
    		currentUserPos.z = user.position.z;
			currentUserPos.z = Mathf.Lerp (user.position.z, newUserPos.z, 2.0 * Time.deltaTime);
			user.position.z = currentUserPos.z; 
		
			if(System.Math.Abs(user.eulerAngles.y-heading)>=5){
    			user.rotation=Quaternion.Slerp (user.transform.rotation,  Quaternion.Euler (0, heading, 0), Time.time * 0.0005);
			}
		}
		
		else{
			//When GPS Emulator is enabled, user position is controlled by keyboard input.
			if(mapping==false){
				//Use keyboard input to move the player
			    if (Input.GetKey ("up") || Input.GetKey ("w")){
					user.transform.Translate(Vector3.forward * speed * Time.deltaTime);
				}
				if (Input.GetKey ("down") || Input.GetKey ("s")){
					user.transform.Translate(-Vector3.forward * speed * Time.deltaTime);
				}
				//rotate pointer when pressing Left and Right arrow keys
				user.Rotate(Vector3.up, Input.GetAxis("Horizontal") * 80 * Time.deltaTime);
			}
		}	
	}
	
	if(mapping){
		//get download progress while images are still downloading
		if(www!=null){download = www.progress;}
	}			
}


//SAMPLE USER INTERFACE. MODIFY OR EXTEND IF NECESSARY.
function OnGUI () {

	if (ready && !mapping && buttons){
		GUI.BeginGroup (Rect (0,screenY-screenY/12, screenX, screenY/12));
			
		GUI.Box (Rect (0,0,screenX,screenY/12), "");
		//Map type toggle button
		if (GUI.Button(new Rect(0,0,screenX/5,screenY/12), maptype[index])){
			if(mapping==false){
				if(index<maptype.Length-1)
		    		index=index+1;
		    	else
		    		index=0;	
		     	MapPosition();
		     	ReScale();
			}    
		}
		//Zoom Out button
		if (GUI.Button(Rect(screenX/5,0,screenX/5,screenY/12), "zoom -")){
			if(zoom>minZoom){
				zoom=zoom-1;
				MapPosition();
				ReScale();
			}
		}
		//Zoom In button
		if (GUI.Button(Rect(2*screenX/5,0,screenX/5,screenY/12), "zoom +")){
			if(zoom<maxZoom){
				zoom=zoom+1;
				MapPosition();
				ReScale();
			}
		}
		//Update map and center user position 
		if (GUI.Button(Rect(3*screenX/5,0,screenX/5,screenY/12), "refresh")){;
			MapPosition();
			ReScale();
		}
		//Show GPS Status info. Please make sure the GPS-Status.js script is attached and enabled in the map object.
		if (GUI.Button(Rect(4*screenX/5,0,screenX/5,screenY/12), "info")){
			if(info)
				info=false;
			else
				info=true;
		}
		GUI.EndGroup ();
	}
}

//Translate decimal latitude to Degrees Minutes and Seconds
function convertdmsLat( lat : float) : String{
	var latAbs = Mathf.Abs(Mathf.Round(lat * 1000000));
    var result : String;
    result = (Mathf.Floor(latAbs / 1000000) + '° '
    		 + Mathf.Floor(((latAbs/1000000) - Mathf.Floor(latAbs/1000000)) * 60)  + '\' '
    	     + (Mathf.Floor(((((latAbs/1000000) - Mathf.Floor(latAbs/1000000)) * 60) - Mathf.Floor(((latAbs/1000000) - Mathf.Floor(latAbs/1000000)) * 60)) * 100000) *60/100000 ).ToString("F2") + '" ')+ ((lat > 0) ? "N" : "S");
	return result;
}   
//Translate decimal longitude to Degrees Minutes and Seconds  
function convertdmsLon( lon:float):String{
	var lonAbs = Mathf.Abs(Mathf.Round(lon * 1000000));
    var result:String; 
    result = (Mathf.Floor(lonAbs / 1000000) + '° ' 
      		 + Mathf.Floor(((lonAbs/1000000) - Mathf.Floor(lonAbs/1000000)) * 60)  + '\' ' 
      		 + (Mathf.Floor(((((lonAbs/1000000) - Mathf.Floor(lonAbs/1000000)) * 60) - Mathf.Floor(((lonAbs/1000000) - Mathf.Floor(lonAbs/1000000)) * 60)) * 100000) *60/100000 ).ToString("F2") + '" ' + ((lon > 0) ? "E" : "W") );
	return result;
}   
