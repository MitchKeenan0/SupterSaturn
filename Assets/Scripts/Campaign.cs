using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaign : MonoBehaviour
{
	private Game game;
	private Camera cameraMain;
	private FleetHUD fleetHud;
	private CampaignBattle campaignBattle;
	private LocationManager locationManager;
	private List<CampaignLocation> locationList;
	private List<Fleet> fleetList;
	private bool bLocationsInit = false;

	void Awake()
    {
		locationList = new List<CampaignLocation>();
		fleetList = new List<Fleet>();
		cameraMain = Camera.main;
		game = FindObjectOfType<Game>();
		game.LoadGame();
		fleetHud = FindObjectOfType<FleetHUD>();
		campaignBattle = FindObjectOfType<CampaignBattle>();
		locationManager = FindObjectOfType<LocationManager>();
    }

	public void InitFleetLocations()
	{
		if (!bLocationsInit)
		{
			bLocationsInit = true;
			locationList = locationManager.GetAllLocations();
			float closestDistance = 999;
			float furthestDistance = 0;
			CampaignLocation myStartLocation = null;
			CampaignLocation enemyStartLocation = null;
			foreach (CampaignLocation cl in locationList)
			{
				if (cl.GetNeighbors().Count > 0)
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
			}

			fleetList = new List<Fleet>(fleetHud.GetFleetList());
			int numFleets = fleetList.Count;
			if (numFleets > 0)
			{
				for (int i = 0; i < numFleets; i++)
				{
					Fleet f = fleetList[i];
					if ((f.teamID == 0) && (myStartLocation != null))
						f.SetLocation(myStartLocation, true);
					if ((f.teamID != 0) && (enemyStartLocation != null))
						f.SetLocation(enemyStartLocation, true);
				}
			}
		}
	}
}
