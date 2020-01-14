using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationDisplay : MonoBehaviour
{
	public GameObject displayPanel;
	public Text nameText;
	public Text valueText;

	private CampaignLocation displayLocation;

	void Start()
    {
		displayPanel.SetActive(false);
	}

	public void SetDisplayLocation(CampaignLocation location)
	{
		if (location != null)
		{
			displayPanel.SetActive(true);
			displayLocation = location;

			nameText.text = location.locationName;
			valueText.text = location.locationValue.ToString();
		}
		else
		{
			displayPanel.SetActive(false);
		}
	}

	public void Close()
	{
		SetDisplayLocation(null);
	}
}
