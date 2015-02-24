using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
public class AndroidPlugin : MonoBehaviour {
	public Text text;
	public Text textDistance;
	public Text textTime;
	public float currentTime = 0;
	public bool running = true;
	private ActivityInfo activity = new ActivityInfo();
	public Button googleLogin;
	public Button googleLogout;
	public Button googleAchievements;
	public Button googleLeadersBoard;
	#if UNITY_ANDROID
	private static AndroidJavaObject androidPlugin = null;
	private static AndroidJavaObject activityContext = null;
	public static Text staticText;
	#endif
	// Use this for initialization
	void Start () {
		GameThrive.Init("7159e91c-aae9-11e4-a836-1b616324191f", "1038037127124", HandleNotification, true);
		ActivityInfo currentActivity = DataStore.LoadLastActivity ();
		if (currentActivity != null) {
			activity = currentActivity;
			textDistance.text = ""+activity.distance+" KM";
			textTime.text = ""+"" +getValue ((int)(activity.time/3600))+":"+ getValue((int)(activity.time/60)%60)+":"+getValue ((int)(activity.time%60));
			currentTime = activity.time;

		}
		if (googleLogout != null) {
			googleLogout.gameObject.SetActive (false);
		}
		if (googleAchievements != null) {
			googleAchievements.gameObject.SetActive (false);
		}
		if (googleLeadersBoard != null) {
			googleLeadersBoard.gameObject.SetActive (false);
		}
		#if UNITY_ANDROID && !UNITY_EDITOR
		staticText = text;
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
								activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>{
							androidPlugin.Call("setContext", activityContext);
							androidPlugin.Call("setCurrentDistance", activity.distance);
						}));
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
	public void showLeadersBoard()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("showLeadersBoardUI");
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
	public void showAchievements()
	{
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("showAchievementsUI");
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
	public void signOut()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("signOut");
				googleLogout.gameObject.SetActive (false);
				googleAchievements.gameObject.SetActive (false);
				googleLeadersBoard.gameObject.SetActive (false);
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
	public void signIn()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("signIn");
				googleLogout.gameObject.SetActive (true);
				googleAchievements.gameObject.SetActive (true);
				googleLeadersBoard.gameObject.SetActive (true);
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
	private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive) {
		print("GameControllerExample:HandleNotification");
		print(message);
	}
	public void showSimpleMessage(string message)
	{
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
	public void showSimpleMessage()
	{
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
		currentTime = 0;
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=>{
			androidPlugin.Call("restartPath");
		}));
		#endif
		activity.dateCreated = new DateTime ();
		activity.distance = 0;
		activity.calories = 0;
		activity.steps = 0;
		activity.time = 0;
	}

	public void loadRunningInfo()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(activityContext != null)
		{
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>
		{
			try{
				float distance = androidPlugin.Call<float>("getCurrentDistance");
				textDistance.text = ""+distance+" KM";
				activity.distance = distance;
				DataStore.SaveLastActivity (activity);
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
	public void startActivity()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(activityContext != null)
		{
			activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>
			                                                              {
				try{
					androidPlugin.Call("startActivity");
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
		running = true;
	}
	public void stopActivity()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(activityContext != null)
		{
			activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>
			                                                              {
				try{
					androidPlugin.Call("stopActivity");
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
		running = false;
	}
	// Update is called once per frame
	void FixedUpdate () {
		if (running) {
			currentTime += Time.deltaTime;
		}
		if (textTime != null) {
						textTime.text = "" + getValue ((int)(currentTime / 3600)) + ":" + getValue ((int)(currentTime / 60) % 60) + ":" + getValue ((int)(currentTime % 60));
		}
		if (activity != null) {
						activity.time = currentTime;
				}
		loadRunningInfo ();
	}
	private string getValue(int value)
	{
		if (value < 10) {
			return "0"+value;
		}
		return ""+value;
	}
}
