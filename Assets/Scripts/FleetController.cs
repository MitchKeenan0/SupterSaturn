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

		if (!locationManager)
			locationManager = FindObjectOfType<LocationManager>();

		List<CampaignLocation> route = new List<CampaignLocation>();
		route = locationManager.GetRouteTo(fleet.GetLocation(), targetLocation);

		lineRenderer.positionCount = 0;
		lineRenderer.positionCount = route.Count;
		int routeSteps = route.Count;
		for(int i = 0; i < routeSteps; i++)
		{
			if (route[i] != null)
				lineRenderer.SetPosition(i, route[i].transform.position);
		}

		lineRenderer.enabled = true;
	}
}
