using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PedometerAdvanced : MonoBehaviour {
	private int peakCount=0;
	private float peakAccumulate = 0;
	private ArrayList list = new ArrayList();
	public float C = 0.8f;
	public float K = 0.1f;
	public Text text;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		peakCount = 0;
		peakAccumulate = 0.0f;
		list.Add (Input.acceleration.magnitude);
		for(int k = 1; k < list.Count-1;k++)
		{
			float forwardSlope = (float)list[k+1]-(float)list[k];
			float backwardSlope = (float)list[k] - (float)list[k-1];
			if(forwardSlope < 0 && backwardSlope > 0)
			{
				peakCount = peakCount+1;
				peakAccumulate = peakAccumulate+(float)list[k];
			}
		}
		float peakMean = peakAccumulate / peakCount;
		float stepCount = 0;
		for(int k = 1; k < list.Count-1;k++)
		{
			float forwardSlope = (float)list[k+1]-(float)list[k];
			float backwardSlope = (float)list[k] - (float)list[k-1];
			if(forwardSlope < 0 && backwardSlope > 0 && (float)list[k] > C*peakMean &&
			   (float)list[k] > K)
			{
				stepCount = peakCount+1;
			}
		}
		text.text = "Steps: "+stepCount;

	}
}
