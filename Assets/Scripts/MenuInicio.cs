using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
public class MenuInicio : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel ("Simple Menu");
		}
		if (Input.GetKeyDown (KeyCode.Menu)) {
			Application.LoadLevel ("Simple Menu");
		}
	}
	
	public void loadMainMenu()
	{
		Application.LoadLevel ("Simple Menu");
	}
	
	public void loadMenuRunning()
	{
		Application.LoadLevel ("RunningUI");
	}
	public void loadMissions()
	{
		Application.LoadLevel ("GooglePlay");
	}
}
