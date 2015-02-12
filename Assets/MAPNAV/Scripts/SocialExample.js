import UnityEngine.SocialPlatforms;

function Start () {
    // Authenticate and register a ProcessAuthentication callback
    // This call needs to be made before we can proceed to other calls in the Social API
    Social.localUser.Authenticate (ProcessAuthentication);
}

// This function gets called when Authenticate completes
// Note that if the operation is successful, Social.localUser will contain data from the server. 
function ProcessAuthentication (success: boolean) {
    if (success) {
        Debug.LogWarning ("Authenticated, checking achievements"+Social.localUser.authenticated);
        // Request loaded achievements, and register a callback for processing them
        createAchievement();
        Social.LoadAchievements (ProcessLoadedAchievements);
        
    }
    else
        Debug.LogWarning ("Failed to authenticate");
}

// This function gets called when the LoadAchievement call completes
function ProcessLoadedAchievements (achievements: IAchievement[]) {
    if (achievements.Length == 0)
        Debug.LogWarning ("Error: no achievements found");
    else
        Debug.LogWarning ("Got " + achievements.Length + " achievements");
    
    // You can also call into the functions like this
    Social.ReportProgress ("Achievement01", 100.0, function(result) {
        if (result)
            Debug.LogWarning ("Successfully reported achievement progress");
        else
            Debug.LogWarning ("Failed to report achievement");
    });
}
function result()
{
	if (result)
		Debug.Log ("Successfully reported progress");
	else
		Debug.Log ("Failed to report progress");
}
function createAchievement()
{
	var achievement:IAchievement = Social.CreateAchievement();
	achievement.id = "Achievement01";
	achievement.percentCompleted = 100.0;
	achievement.ReportProgress(result);
}
