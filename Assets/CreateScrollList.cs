using UnityEngine;
using System.Collections;

public class CreateScrollList : MonoBehaviour {
	public GameObject activityButton;
	public ActivityList activityList;
	public Transform contentPanel;
	public bool test;

	// Use this for initialization
	void Start () {
		populateList ();
	}
	private void populateList (){

		foreach (var item in activityList.activities) {
			GameObject newButton = Instantiate(activityButton) as GameObject;
			ActivityButton button = newButton.GetComponent<ActivityButton>();
			button.dateText.text = item.startDate.ToString();
			button.distanceText.text = item.distance.ToString();
			button.button.onClick.AddListener( ()=>{AndroidPlugin.showSimpleMessage("This activity occurs on "+item.startDate.ToString());});
			newButton.transform.SetParent(contentPanel);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
