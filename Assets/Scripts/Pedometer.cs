using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pedometer : MonoBehaviour {
	/**
	 * Limite inferior de la magnitud de la aceleracion.
	 */
	public float lowLimit = 0.05f;
	/**
	 * Limite en el que se detecta un paso.
	 */
	public float highLimit = 0.1f;

	public int steps = 0;
	private bool stateH = false;
	public float fHigh = 10.0f;
	public float fLow = 0.1f;
	private float curAcc = 0.0f;
	private float avgAcc = 0.0f;
	public Text text;
	private float runningTime;

	// Use this for initialization
	void Start () {
		avgAcc = Input.acceleration.magnitude;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	void FixedUpdate()
	{
		text.text = "Acceleration Magnitude: " + Input.acceleration.magnitude;
		curAcc = Mathf.Lerp (curAcc, Input.acceleration.magnitude, Time.deltaTime * fHigh);
		avgAcc = Mathf.Lerp (avgAcc, Input.acceleration.magnitude, Time.deltaTime * fLow);
		float delta = curAcc - avgAcc;
		
		text.text += "CurAcc = " + curAcc + " AvgAcc = " + avgAcc+" delta = "+delta+" Steps: "+steps;
		if (!stateH) {
			if (delta > highLimit) {
				stateH = true;
				steps++;
			}
		} else {
			if(delta < lowLimit)
			{
				stateH = false;
			}
		}
	}
	// Update is called once per frame
	void Update () {

	}
}
