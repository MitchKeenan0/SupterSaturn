using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignLocation : MonoBehaviour
{
	public int locationID = 0;
	public float reach = 10f;
	public int connections = 2;
	public string locationName = "Name";
	public int locationValue = 1;
	public int rarity = 0;
	public GameObject linePrefab;
	public Material defaultLineMaterial;
	public Material moveLineMaterial;
	public bool bRouteHit = false;

	private NameLibrary nameLibrary;
	private List<LineRenderer> lineList;

	private List<CampaignLocation> connectedLocations;
	public List<CampaignLocation> GetNeighbors() { return connectedLocations; }

	void Awake()
	{
		lineList = new List<LineRenderer>(connections);
		connectedLocations = new List<CampaignLocation>(connections);
	}

	public void InitLocation()
	{
		InitName();
		InitConnections();
	}

	void InitConnections()
	{
		for (int i = 0; i < connections; i++)
		{
			CampaignLocation connection = ConnectToClosestLocation();
			if (connection != null)
				connection.AddConnection(this, false);
		}

		int numLines = lineList.Count;
		if (numLines > 0)
		{
			for (int i = 0; i < numLines; i++)
			{
				LineRenderer thisLine = lineList[i];
				if ((i < connectedLocations.Count) && (connectedLocations[i].transform.position != Vector3.zero))
				{
					thisLine.SetPosition(0, transform.position);
					thisLine.SetPosition(1, connectedLocations[i].transform.position);
					thisLine.gameObject.SetActive(true);
				}
			}
		}
	}

	CampaignLocation ConnectToClosestLocation()
	{
		CampaignLocation connection = null;
		Collider[] cols = Physics.OverlapSphere(transform.position, reach);

		float closestDistance = Mathf.Infinity;
		CampaignLocation closestLocation = null;
		foreach (Collider c in cols)
		{
			if (c.transform != this.transform)
			{
				GameObject colGameObject = c.gameObject;
				float distanceToObject = Vector3.Distance(colGameObject.transform.position, transform.position);
				if (distanceToObject <= closestDistance)
				{
					CampaignLocation location = colGameObject.GetComponent<CampaignLocation>();
					if ((location != null) && (!connectedLocations.Contains(location)))
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
		lineObject.transform.SetParent(transform);
		lineList.Add(liner);
		lineObject.SetActive(false);
	}

	public bool AddConnection(CampaignLocation c, bool executive)
	{
		bool bConnected = false;
		if (((c != this) && !connectedLocations.Contains(c) && (connectedLocations.Count < connections))
			|| executive)
		{
			connectedLocations.Add(c);
			c.AddConnection(this, false);
			bConnected = true;
		}
		if (bConnected)
			CreateLineRenderer();
		return bConnected;
	}

	public void GetScouted(CampaignLocation origin, GameObject connectionPrefab)
	{
		connectedLocations.Add(origin);
		origin.AddConnection(this, true);

		GameObject scoutConnectionLine = Instantiate(connectionPrefab, transform.position, Quaternion.identity);
		LineRenderer connectionLine = scoutConnectionLine.GetComponent<LineRenderer>();
		lineList.Add(connectionLine);
		connectionLine.SetPosition(0, transform.position);
		connectionLine.SetPosition(1, origin.transform.position);
	}

	public void Rename(string value)
	{
		locationName = value;
	}

	public void ConnectionsLit(bool balue)
	{
		Material connectionMat = defaultLineMaterial;
		if (balue)
			connectionMat = moveLineMaterial;
		if (lineList != null)
		{
			foreach (LineRenderer line in lineList)
				line.material = connectionMat;
		}
	}
}
