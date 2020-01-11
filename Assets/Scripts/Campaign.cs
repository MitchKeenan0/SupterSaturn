using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaign : MonoBehaviour
{
	private List<CampaignLocation> locationList;
	private Camera cameraMain;
	private FleetHUD fleetHud;

	void Start()
    {
		locationList = new List<CampaignLocation>();
		cameraMain = Camera.main;
		fleetHud = FindObjectOfType<FleetHUD>();
    }

	public void InitFleetLocations(List<CampaignLocation> locations)
	{
		locationList = locations;
		float closestDistance = 999;
		float furthestDistance = 0;
		CampaignLocation myStartLocation = null;
		CampaignLocation enemyStartLocation = null;
		foreach(CampaignLocation cl in locationList)
		{
			float distanceToLocation = Vector3.Distance(cl.transform.position, cameraMain.transform.position);
			if (distanceToLocation < closestDistance)
			{
				closestDistance = distanceToLocation;
				myStartLocation = cl;
			}
			if (distanceToLocation > furthestDistance)
			{
				furthestDistance = distanceToLocation;
				enemyStartLocation = cl;
			}
		}

		List<Fleet> fleetList = fleetHud.GetFleetList();
		foreach(Fleet f in fleetList)
		{
			if ((f.teamID == 0) && (myStartLocation != null))
				f.SetLocation(myStartLocation);
			if ((f.teamID != 0) && (enemyStartLocation != null))
				f.SetLocation(enemyStartLocation);
		}
	} 
}
