using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identity : MonoBehaviour
{
	public string identityName = "No Name";
	public Color identityColor = Color.green;

	private NameLibrary nameLibrary;
	private CampaignLocation currentLocation;
	private float likelyhoodOfAttackingPlayer = 0f;
	private float likelyhoodOfDefendingPlayer = 0f;
	private float identityWealth = 0f;
	private List<Fleet> fleetList;

	void Awake()
    {
		fleetList = new List<Fleet>();
		nameLibrary = FindObjectOfType<NameLibrary>();
		Fleet myFleet = transform.parent.GetComponentInChildren<Fleet>();
		if (nameLibrary != null)
			InitIdentity();
	}

	public void LoadIdentity(string idName, Color idColor)
	{
		identityName = idName;
		identityColor = idColor;
	}

	public void InitIdentity()
	{
		identityName = nameLibrary.GetIdentityName();

		Color color = new Color();
		color.a = 1;

		likelyhoodOfAttackingPlayer = Random.Range(0f, 1f);
		color.r = likelyhoodOfAttackingPlayer;

		likelyhoodOfDefendingPlayer = Random.Range(0f, 1f);
		color.b = likelyhoodOfDefendingPlayer;

		identityWealth = Random.Range(0f, 1f);
		color.g = identityWealth;

		identityColor = color;
	}

	public void AddFleet(Fleet fleet)
	{
		if ((fleetList.Count == 0) || (!fleetList.Contains(fleet)))
		{
			fleetList.Add(fleet);
			fleet.SetIdentity(this);
			if (fleet.GetLocation() != null)
				SetLocation(fleet.GetLocation());
		}
	}

	public List<Fleet> GetFleetList()
	{
		return fleetList;
	}

	public void SetLocation(CampaignLocation location)
	{
		currentLocation = location;
	}

	public CampaignLocation GetLocation()
	{
		return currentLocation;
	}
}
