using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocationInteraction : MonoBehaviour
{
	private CampaignLocation location;
	private LocationHUD locationHud;

	public CampaignLocation GetLocation() { return location; }

	void Awake()
    {
		location = GetComponent<CampaignLocation>();
		locationHud = FindObjectOfType<LocationHUD>();
    }

	public void OnMouseEnter()
	{
		if (Time.time > 1f)
		{
			if (locationHud != null)
				locationHud.Highlight(this);
		}
	}

	public void OnMouseExit()
	{
		if (Time.time > 1f)
		{
			if (locationHud != null)
				locationHud.Deselect();
		}
	}

	private void OnMouseDown()
	{

	}
}
