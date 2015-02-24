using UnityEngine;
using System.Collections;

public class GoogleAPIExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.LogWarning ("Hello here I am");

		StartCoroutine (callGoogleAPI ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public IEnumerator callGoogleAPI(){
		string urlLocal = "https://flowing-diode-414.appspot.com/_ah/api/greetingApi/v1/getGreetings";

		// Start a download of the given URL
		WWW wwwLocal = new WWW(urlLocal); 

		// Wait for download to complete
		float download = (wwwLocal.progress);
		while(!wwwLocal.isDone){
			Debug.LogWarning("Download progress: "+download+"%");
			yield return null;
		}
		//Show download progress and apply texture
		if(!wwwLocal.isDone || string.IsNullOrEmpty(wwwLocal.error)){
			Debug.LogWarning("There was an error on the request");
			yield return new WaitForSeconds (1);

		}
		//Download Error. Switching to offline mode
		else{
			Debug.LogWarning("Everything is going to be Ok.");
		}
		string result = wwwLocal.text;
		Debug.LogWarning (result);

	}
}
