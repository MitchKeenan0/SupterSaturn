using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineMeasurement : MonoBehaviour
{
	public Text distanceText;

    public void SetDistance(float value)
	{
		distanceText.text = value.ToString("F1");
	}
}
