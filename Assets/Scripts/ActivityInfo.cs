using UnityEngine;
using System.Collections;
using System;
[Serializable]
public class ActivityInfo {
	//time in seconds
	public float time;
	//The distance in kilometers
	public float distance;
	//approximated number of steps
	public int steps;
	//approximated number of calories
	public int calories;
	//the creation date of the activity
	public DateTime dateCreated;

}
