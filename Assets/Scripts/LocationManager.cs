using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
	public float spread = 30f;
	public float verticalScale = 0.618f;
	public float minDistanceBetweenLocations = 5f;
	public int locationCount = 12;
	public GameObject[] locationLibrary;

	private Campaign campaign;
	private List<CampaignLocation> allLocations;
	private List<CampaignLocation> route;

	public List<CampaignLocation> GetAllLocations() { return allLocations; }

	void Awake()
	{
		allLocations = new List<CampaignLocation>();
	}

    void Start()
    {
		route = new List<CampaignLocation>();
		campaign = GetComponent<Campaign>();
		if (allLocations.Count == 0)
			InitLocations();
	}

	public void LoadLocations(List<int> locationIDs, List<Vector3> locationPositions, List<string> locationNames)
	{
		int numLocations = locationIDs.Count;
		if (numLocations > 0)
		{
			for (int i = 0; i < numLocations; i++)
			{
				int locationID = locationIDs[i];
				Vector3 locationPosition = locationPositions[i];
				string locationName = locationNames[i];
				GameObject locationObj = Instantiate(locationLibrary[locationID], locationPosition, Quaternion.identity);
				CampaignLocation cl = locationObj.GetComponent<CampaignLocation>();
				cl.Rename(locationName);
				allLocations.Add(cl);
			}
		}
	}

	void InitLocations()
	{
		float concentricRange = spread;
		for (int i = 0; i < locationCount; i++)
		{
			SpawnRandomFrom(locationLibrary, concentricRange);
			concentricRange += (spread / locationCount);
		}
	}

	void SpawnRandomFrom(GameObject[] array, float range)
	{
		int index = Random.Range(0, array.Length);
		GameObject prefab = array[index];
		GameObject g = Instantiate(prefab, RandomPositionOnDisk() * range, Quaternion.identity);
		allLocations.Add(g.GetComponent<CampaignLocation>());
	}

	Vector3 RandomPositionOnDisk()
	{
		Vector3 pos = Vector3.zero;
		bool messy = true;
		float liveSpread = spread;
		while (messy)
		{
			pos = Random.insideUnitSphere * liveSpread;
			pos.y *= verticalScale;
			int numLocations = allLocations.Count;
			bool hitAny = false;
			for (int i = 0; locationCount < numLocations; i++)
			{
				Vector3 toPos = allLocations[i].transform.position - pos;
				if (toPos.magnitude < minDistanceBetweenLocations)
				{
					hitAny = true;
					liveSpread *= 1.05f;
				}
			}
			messy = hitAny;
		}
		
		return pos;
	}

	public List<CampaignLocation> GetRouteTo(CampaignLocation start, CampaignLocation destination)
	{
		CampaignLocation current = start;
		if (current == destination)
			return null;

		route.Clear();
		route.Add(start);

		foreach (CampaignLocation cl in allLocations)
			cl.bRouteHit = false;

		int safeIndex = 0;
		while (current != destination)
		{
			CampaignLocation nextStep = GetNextNeighbor(current, destination);
			if (nextStep != null)
			{
				route.Add(nextStep);
				current = nextStep;
			}
			else
			{
				///Debug.Log("Destination is unreachable");
				break;
			}

			safeIndex++;
			if (safeIndex >= 100)
				break;
		}

		return route;
	}

	CampaignLocation GetNextNeighbor(CampaignLocation start, CampaignLocation destination)
	{
		CampaignLocation closestLocation = null;

		if ((start != null) && (start.GetNeighbors() != null) && (start.GetNeighbors().Count > 0))
		{
			float closestDistance = Mathf.Infinity;
			List<CampaignLocation> neighbors = new List<CampaignLocation>(start.GetNeighbors());
			int numNeighbors = neighbors.Count;
			start.bRouteHit = true;

			for (int i = 0; i < numNeighbors; i++)
			{
				CampaignLocation cl = neighbors[i];
				if (cl == destination)
					return cl;

				if (cl != start && !cl.bRouteHit)
				{
					float distanceToDestination = Vector3.Distance(cl.transform.position, destination.transform.position);
					if (distanceToDestination < closestDistance)
					{
						closestDistance = distanceToDestination;
						closestLocation = cl;
					}
				}
			}
		}

		if (closestLocation != null)
		{
			closestLocation.bRouteHit = true;
			//Debug.Log("closest location " + closestLocation.locationName);
		}
		else
		{
			//Debug.Log("closest location null");
		}

		return closestLocation;
	}

	public bool IsConnectedByRoute(CampaignLocation start, CampaignLocation destination)
	{
		CampaignLocation current = start;
		if (start == destination)
			return false;

		foreach (CampaignLocation cl in allLocations)
			cl.bRouteHit = false;

		int safeIndex = 0;
		while (current != destination)
		{
			CampaignLocation nextStep = GetNextNeighbor(current, destination);
			if (nextStep != null)
			{
				route.Add(nextStep);
				current = nextStep;
			}
			else
			{
				return false;
			}

			safeIndex++;
			if (safeIndex >= 100)
			{
				Debug.Log("route search hit limit");
				return false;
			}
		}

		return true;
	}
}
