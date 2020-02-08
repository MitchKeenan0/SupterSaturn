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
	private List<CampaignLocation> originLocations = new List<CampaignLocation>();
	private bool bLocationsInit = false;

	void Awake()
    {
		locationList = new List<CampaignLocation>();
		originLocations = new List<CampaignLocation>();
		fleetList = new List<Fleet>();
		cameraMain = Camera.main;
		game = FindObjectOfType<Game>();
		game.LoadGame();
		fleetHud = FindObjectOfType<FleetHUD>();
		campaignBattle = FindObjectOfType<CampaignBattle>();
		locationManager = FindObjectOfType<LocationManager>();
    }

	public void InitFleetLocation(Fleet fleet)
	{
		locationList = locationManager.GetAllLocations();
		fleetList = fleetHud.GetFleetList();
		int numFleets = fleetList.Count;
		for (int i = 0; i < numFleets; i++)
		{
			CampaignLocation furthestLocation = null;
			float furthestDistance = 0f;
			foreach (CampaignLocation cl in locationList)
			{
				if ((cl.GetNeighbors().Count > 0) && (!originLocations.Contains(cl)))
				{
					float distanceToLocation = Vector3.Distance(cl.transform.position, Vector3.zero);
					if (distanceToLocation > furthestDistance)
					{
						furthestLocation = cl;
						furthestDistance = distanceToLocation;
					}
				}
			}
			if (furthestLocation != null)
			{
				fleetList[i].SetLocation(furthestLocation, true);
				originLocations.Add(furthestLocation);
			}
		}
	}
}
