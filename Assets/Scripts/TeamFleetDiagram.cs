using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamFleetDiagram : MonoBehaviour
{
	public Text classText;
	public Text nameText;
	public Text speedText;

	public void SetSpeed(string value)
	{
		speedText.text = value;
	}
}
