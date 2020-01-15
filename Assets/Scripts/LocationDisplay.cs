using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationDisplay : MonoBehaviour
{
	public GameObject displayPanel;
	public Text nameText;
	public Text valueText;

	private CampaignLocation displayLocation = null;
	private Fleet playerFleet = null;

	void Start()
    {
		displayPanel.SetActive(false);
	}

	public void SetPlayerFleet(Fleet fleet)
	{
		playerFleet = fleet;
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

	public void MoveTo()
	{
		if ((displayLocation != null) && (playerFleet != null))
		{
			playerFleet.StandbyMove(displayLocation);
		}
	}

	public void Close()
	{
		SetDisplayLocation(null);
	}
}
