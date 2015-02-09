using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public class AndroidPlugin : MonoBehaviour {
	public Text text;
	#if UNITY_ANDROID
	private static AndroidJavaObject androidPlugin = null;
	private static AndroidJavaObject activityContext = null;
	public static Text staticText;
	#endif
	// Use this for initialization
	void Start () {
		GameThrive.Init("7159e91c-aae9-11e4-a836-1b616324191f", "1038037127124", HandleNotification, true);
		staticText = text;
		#if UNITY_ANDROID && !UNITY_EDITOR
		try{
			if (androidPlugin == null) {
				using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
				using(AndroidJavaClass pluginClass = new AndroidJavaClass("udea.telesalud.artica.com.plugin.AndroidPlugin"))
				{
					if(pluginClass != null)
					{
						androidPlugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
								activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>{androidPlugin.Call("setContext", activityContext);}));
					}
				}
			}
		}
		catch(UnityException ex){
			staticText.text = ex.Message.ToString();
		}
		catch(AndroidJavaException ex)
		{
			staticText.text = ex.Message.ToString();
		}
		#endif
	}

	private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive) {
		print("GameControllerExample:HandleNotification");
		print(message);
	}
	public static void showSimpleMessage(string message)
	{
		load ();
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("showMessage", message);
			}
			catch(UnityException ex){
				staticText.text = ex.Message.ToString();
			}
			catch(AndroidJavaException ex)
			{
				staticText.text = ex.Message.ToString();
			}
			
		}));
		#endif
	}
	public static void showSimpleMessage()
	{
		load ();
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
		()=> 
		{
			try{
				androidPlugin.Call("showMessage", "This is a very important messagge");
			}
			catch(UnityException ex){
				staticText.text = ex.Message.ToString();
			}
			catch(AndroidJavaException ex)
			{
				staticText.text = ex.Message.ToString();
			}
			
		}));
		#endif
	}
	public void restartActivity()
	{
		save ();
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=>{
			androidPlugin.Call("restartPath");
		}));
		#endif

	}
	/**
	 * Plantilla para almacenar datos en la base en archivo dentro del juego.
	 */
	public static void save()
	{
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath+"/playerInfo.path");
		RunActivity data = new RunActivity ();
		data.someData = "Some Data";
		formatter.Serialize (file, data);
		file.Close ();
	}
	/**
	 * Plantilla para cargar la informacion almacenada.
	 */
	public static void load()
	{
		if (File.Exists (Application.persistentDataPath + "/playerInfo.path")) {
			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath+"/playerInfo.path",  FileMode.Open);
			RunActivity data = (RunActivity)formatter.Deserialize(file);
			file.Close();
			#if UNITY_ANDROID && !UNITY_EDITOR
			activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
				()=> 
				{
				try{
					androidPlugin.Call("showMessage", data.someData);
				}
				catch(UnityException ex){
					staticText.text = ex.Message.ToString();
				}
				catch(AndroidJavaException ex)
				{
					staticText.text = ex.Message.ToString();
				}
				
			}));
			#endif
		}
	}
	public void showToast()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(activityContext != null)
		{
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>
		{
			try{
				float distance = androidPlugin.Call<float>("getCurrentDistance");
				long time = androidPlugin.Call<long>("getCurrentTime");
				string location = androidPlugin.Call<string>("getLocation");
					staticText.text = "Distancia = "+distance+" meters, Tiempo: "+time+" milliseconds. Location: "+location;
			}
			catch(UnityException ex){
					staticText.text = ex.Message.ToString();
			}
			catch(AndroidJavaException ex)
			{
					staticText.text = ex.Message.ToString();
			}

		}));
		}
		#endif
	}
	// Update is called once per frame
	void Update () {
		#if UNITY_ANDROID && !UNITY_EDITOR
		try{
			showToast();
		}
		catch(UnityException ex){
			staticText.text = ex.Message.ToString();
		}
		catch(AndroidJavaException ex)
		{
			staticText.text = ex.Message.ToString();
		}
		#endif
	}
}
