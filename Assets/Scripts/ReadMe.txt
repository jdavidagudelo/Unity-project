// Must Publish to Emulator or Android Device To Test Demo Works with Level 9+

// Support information for AdMobs
//(Demo:Hard Coded Public ID) Purchase for Full Version
/**
 * MobAds supports Android build level 9+
 * JAVA:startAds(String pub_id, final int adtype, 
 * 			final String pos_y, final String pos_x);
   *Ad Types
	   0 SMART_BANNER
	   1 BANNER
	   2 IAB_BANNER
	   3 IAB_LEADERBOARD
	   4 IAB_WIDE_SKYSCRAPER		            	
   *Positions
	   position y: top, middle, bottom
  	   position x: left, center, right
   *Example   
   	   AndroidPlugins.StartAds("YOUR_PUBLIC_ADMOB_ID", 1, "top", "left");// to start the ads
   *Refreshing Ads put in your Start() method on your MonoBehaviour Class
	   float refreshSeconds=10f;
	   InvokeRepeating("RefreshAds", 0f, refreshSeconds);
	   // Add this method to the class you call the Android.StartAds 
	   void RefreshAds(){
	       AndroidPlugins.ReAds();
	   }
   *Stop the Ads
   	   StopAds();   
*/

// Support Information for Google Wallet In App Purchases
/* (DEMO: Hard Coded Item and Publisher ID Purchase the full version for 
 * your item testing)
 * Google Wallet Android Plugin In App Purchases
 * Setup Your In App Payments for your Android App
 * Each In App Payment Type Needs to be Managed
 * Fill in the form then activiate the item
 * Once Play has updated then you can use
 	AndroidPlugins.BuyItem("SKUID", "SUCCESS MESSAGE", "Game Object Name for Callback", "Game Object Method for Callback");
 	As soon as the purchase is finished it will call your method the sku id that was purchased
 * If you want to look up details about a skuid then
 	AndroidPlugins.BuyItem("SKUID", "SUCCESS MESSAGE", "Game Object Name for Callback", "Game Object Method for Callback");
  	will return to you a String of the details of the item seperated by a ";"
  /

// Support information for Notifications
//(DEMO: Can only make 1 request.) Purchase for Full Version
/**
 * Notifications supports Android build level 9+ used with the compatability support
 * JAVA:sendNotification(String subject, String message, boolean withSound, 
 * 						int soundType, boolean vibrate, long[] pattern);  	
 *With Sound/Sound Types
   0 TYPE_NOTIFICATION
   1 TYPE_ALARM
   2 TYPE_RINGTONE
   
  *With Lights/Light Explaination
   onMs=500 // the number of milliseconds to be on while its flashing
   offMs=500 // the number of milliseconds to be off while its flashing
   
   *Vibration Patterns with long[] arrays:
   See Example Method SendNotificationVibrateSOS Below 
   
   *Example
   AndroidPlugins.SendNotificationVibrateSOS("subject", "message");
   
   *Background Service Notification System built to your Servers Requirements available on requests
   *Email details@gandpgaming.com for more details
 */


// Products Available Upon Request
/**
 * Multi-Touch Screen Tools
 * Touch Scroller
 * Touch Buttons
 * Touch Joysticks and Multiple Touch Joysticks on Screen
 * Background Services
 * Email details@gandpgaming.com for more details
 */



