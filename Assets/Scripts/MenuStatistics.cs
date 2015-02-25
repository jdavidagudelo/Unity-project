using UnityEngine;
using System.Collections;

public class MenuStatistics : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel ("Simple Menu");
		}
		if (Input.GetKeyDown (KeyCode.Menu)) {
			Application.LoadLevel ("Simple Menu");
		}
	}
}
