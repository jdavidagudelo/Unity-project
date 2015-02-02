using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class AndroidPlugin : MonoBehaviour {
	public Text text;
	private AndroidJavaObject toastExample = null;
	private AndroidJavaObject activityContext = null;
	// Use this for initialization
	void Start () {
		try{
		if (toastExample == null) {
			using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			using(AndroidJavaClass pluginClass = new AndroidJavaClass("udea.telesalud.artica.com.plugin.ToastExample"))
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
	// Update is called once per frame
	void Update () {
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
	}
}
