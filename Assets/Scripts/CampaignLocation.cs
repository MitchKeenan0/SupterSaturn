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
	public GameObject linePrefab;

	private NameLibrary nameLibrary;
	private List<LineRenderer> lineList;

	private List<CampaignLocation> connectedLocations;
	public List<CampaignLocation> GetNeighbors() { return connectedLocations; }
	public void AddConnection(CampaignLocation c)
	{
		if ((c != this) && !connectedLocations.Contains(c)
			&& (connectedLocations.Count < connections))
			connectedLocations.Add(c);
	}

	void Awake()
    {
		connectedLocations = new List<CampaignLocation>();
		lineList = new List<LineRenderer>();
		InitName();
		InitConnections();
    }

	void InitConnections()
	{
		for (int i = 0; i < connections; i++)
		{
			if (connectedLocations.Count < connections)
			{
				CampaignLocation connection = ConnectToClosestLocation();
				if (connection != null)
				{
					AddConnection(connection);
					connection.AddConnection(this);
					CreateLineRenderer();
				}
			}
		}
		int numConnections = connectedLocations.Count;
		if (numConnections > 0)
		{
			for (int i = 0; i < numConnections; i++)
			{
				LineRenderer thisLine = lineList[i];
				thisLine.SetPosition(0, transform.position);
				thisLine.SetPosition(1, connectedLocations[i].transform.position);
			}
		}
	}

	CampaignLocation ConnectToClosestLocation()
	{
		CampaignLocation connection = null;
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
					if (!connectedLocations.Contains(location) && (location != this)
						&& (location.GetNeighbors().Count < location.connections))
					{
						closestDistance = distanceToObject;
						closestLocation = location;
					}
				}
			}
		}
		if (closestLocation != null)
			connection = closestLocation;
		return connection;
	}

	void InitName()
	{
		nameLibrary = FindObjectOfType<NameLibrary>();
		if (nameLibrary != null)
			locationName = nameLibrary.GetLocationName(rarity);
	}

	void CreateLineRenderer()
	{
		GameObject lineObject = Instantiate(linePrefab, transform.position, Quaternion.identity);
		LineRenderer liner = lineObject.GetComponent<LineRenderer>();
		lineList.Add(liner);
	}
}
