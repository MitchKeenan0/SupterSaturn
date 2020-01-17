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
	private FleetController fleetController;

    void Awake()
    {
		spacecraftList = new List<Spacecraft>();
		fleetController = GetComponent<FleetController>();
		foreach (Spacecraft sp in initialFleet)
			spacecraftList.Add(sp);
		DontDestroyOnLoad(gameObject);
	}

	public void StandbyMove(CampaignLocation location)
	{
		fleetController.SetTargetLocation(location);
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}

	public void SetLocation(CampaignLocation location, bool position)
	{
		campaignLocation = location;
		if (position)
			transform.position = location.transform.position;
	}

	public CampaignLocation GetLocation() { return campaignLocation; }

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
