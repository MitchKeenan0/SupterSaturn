using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetController : MonoBehaviour
{
	private Fleet fleet;
	private LineRenderer lineRenderer;
	private CampaignLocation targetLocation;
	private LocationManager locationManager;

    void Start()
    {
		fleet = GetComponent<Fleet>();
		locationManager = FindObjectOfType<LocationManager>();
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.enabled = false;
    }

	void Move()
	{
		fleet.SetLocation(targetLocation);
	}

	public void SetTargetLocation(CampaignLocation location)
	{
		targetLocation = location;
		lineRenderer.enabled = true;

		if (!locationManager)
			locationManager = FindObjectOfType<LocationManager>();
		List<CampaignLocation> route = locationManager.GetRouteTo(fleet.GetLocation(), targetLocation);

		Debug.Log("Route has " + route.Count + " nodes");

		lineRenderer.positionCount = route.Count;
		int routeSteps = route.Count;
		for(int i = 0; i < routeSteps; i++)
		{
			if (route[i] != null)
				lineRenderer.SetPosition(i, route[i].transform.position);
		}
	}
}
