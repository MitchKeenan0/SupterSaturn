using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivePanel : MonoBehaviour
{
	public Text objectiveText;
	public Text distanceText;

	void Start()
	{
		objectiveText.text = distanceText.text = "";
	}

	public void SetObjective(string value)
	{
		objectiveText.text = value;
	}

	public void SetDistance(float value)
	{
		distanceText.text = value.ToString("F1") + " km";
	}
}
