using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamFleetDiagram : MonoBehaviour
{
	public Text classText;
	public Text nameText;
	public Text speedText;
	public Image gravityIcon;
	public Image violenceIcon;

    void Start()
    {
        
    }

	public void SetClass(string value)
	{
		classText.text = value;
	}

	public void SetName(string value)
	{
		nameText.text = value;
	}

	public void SetSpeed(string value)
	{
		speedText.text = value;
	}

	public void SetGravityIcon(bool value)
	{
		gravityIcon.enabled = value;
	}

	public void SetViolenceIcon(bool value)
	{
		violenceIcon.enabled = value;
	}
}
