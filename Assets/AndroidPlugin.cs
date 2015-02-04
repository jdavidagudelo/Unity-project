using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class AndroidPlugin : MonoBehaviour {
	public Text text;
#if UNITY_ANDROID
	private AndroidJavaObject toastExample = null;
	private AndroidJavaObject activityContext = null;
#endif
	// Use this for initialization
	void Start () {
#if UNITY_ANDROID
		try{
		if (toastExample == null) {
			using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			using(AndroidJavaClass pluginClass = new AndroidJavaClass("udea.telesalud.artica.com.plugin.AndroidPlugin"))
			{
				if(pluginClass != null)
				{
					toastExample = pluginClass.CallStatic<AndroidJavaObject>("instance");
							activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>{toastExample.Call("setContext", activityContext);}));
				}
			}
		}
		}
		catch(UnityException ex){
			text.text = ex.Message.ToString();
		}
		catch(AndroidJavaException ex)
		{
			text.text = ex.Message.ToString();
		}
#endif
	}
#if UNITY_ANDROID
	public void showSimpleMessage()
	{
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>
		                                                              {
			try{
				toastExample.Call("showMessage", "This is a very important messagge");
			}
			catch(UnityException ex){
				text.text = ex.Message.ToString();
			}
			catch(AndroidJavaException ex)
			{
				text.text = ex.Message.ToString();
			}
			
		}));
	}
	public void showToast()
	{

		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>
		{
			try{
				float distance = toastExample.Call<float>("getCurrentDistance");
				long time = toastExample.Call<long>("getCurrentTime");
				string location = toastExample.Call<string>("getLocation");
				text.text = "Distancia = "+distance+" meters, Tiempo: "+time+" milliseconds. Location: "+location;
			}
			catch(UnityException ex){
				text.text = ex.Message.ToString();
			}
			catch(AndroidJavaException ex)
			{
				text.text = ex.Message.ToString();
			}

		}));
	}
#endif
	// Update is called once per frame
	void Update () {
#if UNITY_ANDROID
		try{
			showToast();
		}
		catch(UnityException ex){
			text.text = ex.Message.ToString();
		}
		catch(AndroidJavaException ex)
		{
			text.text = ex.Message.ToString();
		}
#endif
	}
}
