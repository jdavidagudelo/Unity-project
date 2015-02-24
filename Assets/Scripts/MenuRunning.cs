using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MenuRunning : MonoBehaviour {
	public AndroidPlugin androidPlugin;
	public SpeedAndDistance speedAndDistance;
	public bool usePlugin;
	public Text buttonPlayText;
	// Use this for initialization
	void Start () {
		speedAndDistance.enabled = !usePlugin;
		androidPlugin.enabled = usePlugin;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel ("Inicio");
		}
		if (Input.GetKeyDown (KeyCode.Menu)) {
			Application.LoadLevel ("Inicio");
		}
	}
	public void pressButtonPlay()
	{
		if (!usePlugin) {
			if (!speedAndDistance.running) {
				buttonPlayText.text = "PAUSAR";
				speedAndDistance.startActivity ();
		} else {
			buttonPlayText.text = "INICIAR";
			speedAndDistance.stopActivity ();
			}
		} 
		else 
		{
			if (!androidPlugin.running) {
				buttonPlayText.text = "PAUSAR";
				androidPlugin.startActivity ();
			} else {
				buttonPlayText.text = "INICIAR";
				androidPlugin.stopActivity ();
			}
		}
	}
	public void loadMenuInicio()
	{
		Application.LoadLevel ("Inicio");
	}
}
