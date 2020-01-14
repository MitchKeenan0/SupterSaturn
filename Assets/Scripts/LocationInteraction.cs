using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocationInteraction : MonoBehaviour
{
	private CampaignLocation location;
	private LocationHUD locationHud;
	private LocationDisplay locationDisplay;

	public CampaignLocation GetLocation() { return location; }

	void Awake()
    {
		location = GetComponent<CampaignLocation>();
		locationHud = FindObjectOfType<LocationHUD>();
		locationDisplay = FindObjectOfType<LocationDisplay>();
    }

	public void OnMouseEnter()
	{
		if (Time.time > 0.2f)
		{
			if (locationHud != null)
				locationHud.Highlight(this);
		}
	}

	public void OnMouseExit()
	{
		if (Time.time > 0.2f)
		{
			if (locationHud != null)
				locationHud.Deselect();
		}
	}

	private void OnMouseDown()
	{
		if (locationDisplay != null)
			locationDisplay.SetDisplayLocation(location);
	}
}
