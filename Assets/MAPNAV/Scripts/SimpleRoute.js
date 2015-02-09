import SimpleJSON;
private var download : float;
private var www : WWW;
private var url = "https://maps.googleapis.com/maps/api/directions/json?"; 
private var status:String;
public var apiKey = "AIzaSyDWUWNAhTWdoFW4L-4jD-S2aFqOOKhFN7A";
function Start()
{
	getRoute("Toronto", "Montreal", "WALKING");
}
//ONLINE MAP DOWNLOAD
function getRoute(origin : String,  destination: String, travelMode :String){
	url = url +"origin="+origin+"&destination="+destination+"&travelMode="+travelMode+"&sensor=false&key="+apiKey;
	url = "https://maps.googleapis.com/maps/api/directions/json?origin=6.230755,-75.589746&destination=6.226147,-75.596913&travelMode=WALKING&sensor=false&key=AIzaSyDWUWNAhTWdoFW4L-4jD-S2aFqOOKhFN7A";
	var pointsParams : String = "";
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
		var tmp : Texture2D;
		tmp = new Texture2D(1280,1280,TextureFormat.RGB24,false);
		www.LoadImageIntoTexture(tmp); 	
	}
	//Download Error. Switching to offline mode
	else{
		print("Map Error:"+www.error);
		//use the status string variable to print messages to your own user interface (GUIText, etc.)
		status="Map Error:"+www.error;
		yield WaitForSeconds (1);
	}
	var result = JSON.Parse(www.text);
	Debug.LogWarning(result["status"].Value);
	var routes = result["routes"];
	for(var route : JSONNode in routes)
	{
		var legs = route["legs"];
		for(var leg : JSONNode in legs)
		{
			var steps = leg["steps"];
			for(var step :JSONNode in steps)
			{
				var startLocation : JSONNode= step["start_location"];
				var endLocation : JSONNode = step["end_location"];
				pointsParams += startLocation["lat"]+","+startLocation["lng"]+"|";
				pointsParams += endLocation["lat"]+","+endLocation["lng"]+"|";
			}
			break;
		}
		Debug.LogWarning(route["summary"]);
		break;
	}
	Debug.LogWarning(pointsParams.Substring(0, pointsParams.length - 1));
	Debug.LogWarning(url);
	pointsParams = pointsParams.Substring(0, pointsParams.length - 1);
}
