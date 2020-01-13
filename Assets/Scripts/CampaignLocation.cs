using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignLocation : MonoBehaviour
{
	public float reach = 10f;
	public int connections = 2;
	public string locationName = "Name";
	public int locationValue = 1;
	public int rarity = 0;

	private NameLibrary nameLibrary;
	private LineRenderer lineRenderer;
	private List<CampaignLocation> connectedLocations;

    void Start()
    {
		InitName();
		InitConnections();
    }

	void InitConnections()
	{
		connectedLocations = new List<CampaignLocation>();
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.positionCount = 3;
		for (int i = 0; i < lineRenderer.positionCount; i++)
			lineRenderer.SetPosition(i, transform.position);

		for (int i = 0; i < connections; i++)
		{
			GetClosestConnection();
		}

		if (connectedLocations.Count >= 1)
			lineRenderer.SetPosition(0, connectedLocations[0].transform.position);

		lineRenderer.SetPosition(1, transform.position);

		if (connectedLocations.Count > 1)
			lineRenderer.SetPosition(2, connectedLocations[1].transform.position);
	}

	void GetClosestConnection()
	{
		Collider[] cols = Physics.OverlapSphere(transform.position, reach);
		float closestDistance = reach;
		CampaignLocation closestLocation = null;
		foreach (Collider c in cols)
		{
			GameObject colGameObject = c.gameObject;
			float distanceToObject = Vector3.Distance(colGameObject.transform.position, transform.position);
			if (distanceToObject <= closestDistance)
			{
				CampaignLocation location = colGameObject.GetComponent<CampaignLocation>();
				if (location != null)
				{
					if (!connectedLocations.Contains(location))
					{
						closestDistance = distanceToObject;
						closestLocation = location;
						break;
					}
				}
			}
		}
		if (closestLocation != null)
			connectedLocations.Add(closestLocation);
	}

	void InitName()
	{
		nameLibrary = FindObjectOfType<NameLibrary>();
		if (nameLibrary != null)
			locationName = nameLibrary.GetLocationName(rarity);
	}
}
