using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
public class MenuProfile : MonoBehaviour {
	public InputField inputNickname;
	public InputField inputWeight;
	public InputField inputHeight;
	public InputField inputAge;
	public Toggle toggleMale;
	public Toggle toggleFemale;
	private string pattern= "[0-9]+(\\.[0-9]*)?";
	// Use this for initialization
	void Start () {
		UserData data = DataStore.Load ();
		inputNickname.text = "" + data.nickname;
		inputWeight.text = "" + data.weight;
		inputHeight.text = "" + data.height;
		inputAge.text = "" + data.age;
		toggleMale.isOn = data.male;
		toggleFemale.isOn = data.female;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void loadMainMenu()
	{
		Application.LoadLevel ("Simple Menu");
	}
	public void SetUserNickname()
	{
		DataStore.data.nickname = inputNickname.text;
		DataStore.Save ();
	}
	public void SetUserWeight()
	{
		if(Regex.IsMatch(inputWeight.text, pattern))
		{
			DataStore.data.weight = float.Parse(inputWeight.text);
			DataStore.Save ();
		}
	}
	public void SetUserHeight()
	{
		if (Regex.IsMatch (inputHeight.text, pattern)) {
						DataStore.data.height = float.Parse (inputHeight.text);
						DataStore.Save ();
		}
	}
	public void SetGender()
	{
		DataStore.data.female = toggleFemale.isOn;
		DataStore.data.male = toggleMale.isOn;
		DataStore.Save ();
	}
	public void SetUserAge()
	{
		if (Regex.IsMatch (inputAge.text, pattern)) {
						DataStore.data.age = float.Parse (inputAge.text);
						DataStore.Save ();
				}
	}

}
