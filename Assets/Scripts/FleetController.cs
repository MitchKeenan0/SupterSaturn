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
	private LineRenderer lineRenderer;
	private List<CampaignLocation> route;

	private bool bTurnSubmitted = false;
	public bool TurnSubmitted() { return bTurnSubmitted; }
	public void SetTurnSubmitted(bool value) { bTurnSubmitted = value; }

	private IEnumerator fleetMoveCoroutine;
	private bool bMoving = false;

    void Start()
    {
		route = new List<CampaignLocation>();
		fleet = GetComponent<Fleet>();
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
			bMoving = true;
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
		bMoving = false;
		if (targetLocation != null)
			SetTargetLocation(targetLocation);
	}
}
