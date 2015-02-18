using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
public class Validator : MonoBehaviour {
	
	public bool validateNumber(string s)
	{
		return Regex.Match (s, "[0-9]").Success;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
