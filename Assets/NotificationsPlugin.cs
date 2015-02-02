using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NotificationsPlugin : MonoBehaviour {

	public Text text;
	private AndroidJavaObject displayNotifications = null;
	private AndroidJavaObject activityContext = null;
	// Use this for initialization
	void Start () {
		try{
			if (displayNotifications == null) {
				using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
				using(AndroidJavaClass pluginClass = new AndroidJavaClass("udea.telesalud.artica.com.notificationsplugin.DisplayNotifications"))
				{
					if(pluginClass != null)
					{
						displayNotifications = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
						activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>{displayNotifications.Call("setContext", activityContext);}));
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
	public void displayNotificationTwo()
	{
		
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>
		                                                              {
			try{
				displayNotifications.Call("displayNotifications", "This is the message");
			}
			catch(UnityException ex){
				text.text = ex.Message.ToString();
			}
			catch(AndroidJavaException ex)
			{
				text.text = ex.Message.ToString();
			}
			
		}));
		text.text = "Notification 2 called";
	}
	public void displayNotificationOne()
	{
		
		activityContext.Call("runOnUiThread", new AndroidJavaRunnable(()=>
		                                                              {
			try{
				displayNotifications.Call("displayNotificationOne");
			}
			catch(UnityException ex){
				text.text = ex.Message.ToString();
			}
			catch(AndroidJavaException ex)
			{
				text.text = ex.Message.ToString();
			}
			
		}));
		text.text = "Notification 2 called";
	}
	// Update is called once per frame
	void Update () {

	}
}
