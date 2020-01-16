using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetController : MonoBehaviour
{
	public float fleetMoveSpeed = 1.5f;
	public float movementUpdateInterval = 0.02f;

	private Fleet fleet;
	private LocationManager locationManager;
	private CampaignLocation targetLocation;
	private CampaignLocation moveLocation;
	private Campaign campaign;
	private CampaignBattle campaignBattle;
	private LineRenderer lineRenderer;
	private List<CampaignLocation> route;
	private List<Fleet> allFleetsList;

	private bool bTurnSubmitted = false;
	public bool TurnSubmitted() { return bTurnSubmitted; }
	public void SetTurnSubmitted(bool value) { bTurnSubmitted = value; }

	private IEnumerator fleetMoveCoroutine;

    void Start()
    {
		route = new List<CampaignLocation>();
		allFleetsList = new List<Fleet>();

		fleet = GetComponent<Fleet>();
		Fleet[] allFleets = FindObjectsOfType<Fleet>();
		foreach(Fleet f in allFleets)
		{
			if (f != fleet)
				allFleetsList.Add(f);
		}

		campaign = FindObjectOfType<Campaign>();
		campaignBattle = FindObjectOfType<CampaignBattle>();
		locationManager = FindObjectOfType<LocationManager>();
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.enabled = false;
    }

	public void SetTargetLocation(CampaignLocation location)
	{
		targetLocation = location;
		if (!locationManager)
			locationManager = FindObjectOfType<LocationManager>();
		route = locationManager.GetRouteTo(fleet.GetLocation(), targetLocation);

		lineRenderer.positionCount = 0;
		lineRenderer.positionCount = route.Count;
		int routeSteps = route.Count;
		for (int i = 0; i < routeSteps; i++)
		{
			if (route[i] != null)
				lineRenderer.SetPosition(i, route[i].transform.position);
		}

		lineRenderer.enabled = true;
	}

	public void ExecuteTurn()
	{
		if (route.Count > 1)
		{
			moveLocation = route[1];
			fleetMoveCoroutine = FleetMove(movementUpdateInterval);
			StartCoroutine(fleetMoveCoroutine);
		}
	}
	
	private IEnumerator FleetMove(float updateInterval)
	{
		while (fleet.GetLocation() != moveLocation)
		{
			UpdateMove(updateInterval);
			yield return new WaitForSeconds(updateInterval);
		}
	}

	void UpdateMove(float delta)
	{
		Vector3 toMoveLocation = moveLocation.transform.position - transform.position;
		if (toMoveLocation.magnitude > 0.1f)
		{
			Vector3 move = fleet.transform.position + (toMoveLocation.normalized * fleetMoveSpeed * delta);
			fleet.SetPosition(move);
		}
		else
		{
			FinishMove();
		}
	}

	void FinishMove()
	{
		fleet.SetLocation(moveLocation, true);
		if (targetLocation != null)
			SetTargetLocation(targetLocation);

		foreach(Fleet f in allFleetsList)
		{
			CampaignLocation thisLocation = fleet.GetLocation();
			if ((f.GetLocation() == thisLocation) && f.teamID != fleet.teamID)
			{
				Fleet[] parties = new Fleet[] { fleet, f };
				campaignBattle.HeraldBattle(thisLocation, parties);
			}
		}
	}
}
