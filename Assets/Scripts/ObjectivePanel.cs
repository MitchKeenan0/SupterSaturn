using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivePanel : MonoBehaviour
{
	public Text objectiveText;
	public Text distanceText;

	private string objName = "";

	void Start()
	{
		objectiveText.text = distanceText.text = "";
	}

	public void SetObjective(string value)
	{
		objName = value;
		objectiveText.text = objName;
		Debug.Log("set panel text " + objectiveText.text);
	}

	public void SetDistance(float value)
	{
		distanceText.text = value.ToString("F1") + " km";
	}
}
