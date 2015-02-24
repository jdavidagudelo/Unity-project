using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataStore : MonoBehaviour {
	public static UserData data = new UserData ();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public static void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath+"/playerInfo.dat");
		bf.Serialize (file, data);
		file.Close ();
	}
	public static UserData Load()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
		data = (UserData)bf.Deserialize (file);
		file.Close ();
		return data;
	}
	public static void SaveLastActivity(ActivityInfo activity)
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath+"/lastActivityInfo.dat");
		bf.Serialize (file, activity);
		file.Close ();
	}
	public static ActivityInfo LoadLastActivity()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		if(File.Exists(Application.persistentDataPath + "/lastActivityInfo.dat"))
		{
			FileStream file = File.Open (Application.persistentDataPath + "/lastActivityInfo.dat", FileMode.Open);
			ActivityInfo info = (ActivityInfo)bf.Deserialize (file);
			file.Close ();
			return info;
		}
		return null;
	}
}
