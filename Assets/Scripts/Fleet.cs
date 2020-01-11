using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet : MonoBehaviour
{
	public int teamID = 0;
	public Spacecraft[] initialFleet;
	public string fleetName = "My Fleet";

	private List<Spacecraft> spacecraftList;
	private CampaignLocation campaignLocation;

    void Awake()
    {
		spacecraftList = new List<Spacecraft>();
		foreach (Spacecraft sp in initialFleet)
			spacecraftList.Add(sp);
	}

	public void SetLocation(CampaignLocation location)
	{
		campaignLocation = location;
		transform.position = campaignLocation.transform.position;
	}

	public List<Spacecraft> GetSpacecraftList()
	{
		return spacecraftList;
	}

	public void AddSpacecraft(Spacecraft sp)
	{
		if (!spacecraftList.Contains(sp))
			spacecraftList.Add(sp);
	}

	public void RemoveSpacecraft(Spacecraft sp)
	{
		if (spacecraftList.Contains(sp))
			spacecraftList.Remove(sp);
	}
}
