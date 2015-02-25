using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
public class GooglePlayServices : MonoBehaviour {
	public Text text;
	public Button googleLogin;
	public Button googleLogout;
	public Button googleAchievements;
	public Button googleLeadersBoard;
	public Button googleQuests;
	public Button googleInbox;
	public Button googleGifts;
	public Button googleRequests;
	private string savedData = null;
	#if UNITY_ANDROID
	private static AndroidJavaObject androidPlugin = null;
	private static AndroidJavaObject activityContext = null;
	public static Text staticText;
	#endif
	void Start () {
		GameThrive.Init("7159e91c-aae9-11e4-a836-1b616324191f", "1038037127124", HandleNotification, true);

		if (googleLogout != null) {
			googleLogout.gameObject.SetActive (false);
		}
		if (googleAchievements != null) {
			googleAchievements.gameObject.SetActive (false);
		}
		if (googleLeadersBoard != null) {
			googleLeadersBoard.gameObject.SetActive (false);
		}
		if (googleQuests != null) {
			googleQuests.gameObject.SetActive(false);
		}
		if (googleInbox != null) {
			googleInbox.gameObject.SetActive(false);
		}
		
		if (googleGifts!= null) {
			googleGifts.gameObject.SetActive(false);
		}
		if (googleRequests != null) {
			googleRequests.gameObject.SetActive(false);
		} 
		#if UNITY_ANDROID && !UNITY_EDITOR
		staticText = text;
		try{
			if (androidPlugin == null) {
				using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
				using(AndroidJavaClass pluginClass = new AndroidJavaClass("com.artica.juegohabitos.androidplugins.GooglePlayServicesPlugin"))
				{
					if(pluginClass != null)
					{
						androidPlugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
						activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>{
							androidPlugin.Call("setContext", activityContext);
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
	
	private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive) {
		print("GameControllerExample:HandleNotification");
		print(message);
	}
	public void loadSavedData()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				savedData = androidPlugin.Call<string>("getSavedGamesData");
				staticText.text = savedData;
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
	public void savedGamesLoad(string snapshotName)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("loadSavedGame",  snapshotName);
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
	public void savedGamesUpdate(string data, string snapshotName, bool createIfMissing)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("savedGamesUpdate", data, snapshotName, createIfMissing);
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
	public void showQuests()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("showQuests");
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
	public void showInbox()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("showInbox");
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
	public void showRequests()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("showRequestsUI");
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
	public void showGifts()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(
			()=> 
			{
			try{
				androidPlugin.Call("showGiftsUI");
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
				googleQuests.gameObject.SetActive (false);
				googleInbox.gameObject.SetActive (false);
				googleRequests.gameObject.SetActive (false);
				googleGifts.gameObject.SetActive (false);
				googleLogin.gameObject.SetActive(true);
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
				googleQuests.gameObject.SetActive (true);
				googleInbox.gameObject.SetActive (true);
				googleRequests.gameObject.SetActive (true);
				googleGifts.gameObject.SetActive (true);
				googleLogin.gameObject.SetActive(false);
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

	// Update is called once per frame
	void Update () {
		if (savedData == null) {
			
			savedGamesUpdate ("This is my stupid test", "SnapShot1", true);
			savedGamesLoad ("SnapShot1");
		}
		loadSavedData ();
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel ("Inicio");
		}
		if (Input.GetKeyDown (KeyCode.Menu)) {
			Application.LoadLevel ("Inicio");
		}
	}
}
