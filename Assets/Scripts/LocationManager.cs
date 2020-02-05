using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
	public float spread = 30f;
	public float minDistanceBetweenLocations = 5f;
	public int locationCount = 12;
	public GameObject[] commonLocations;
	public GameObject[] uniqueLocations; 
	public GameObject[] rareLocations;

	private Campaign campaign;
	private List<CampaignLocation> allLocations;
	private List<CampaignLocation> route;

	public List<CampaignLocation> GetAllLocations() { return allLocations; }

    void Start()
    {
		route = new List<CampaignLocation>();
		campaign = GetComponent<Campaign>();
		InitLocations();
		///Debug.Log("Finished location manager start");
    }

	void InitLocations()
	{
		allLocations = new List<CampaignLocation>();
		float concentricRange = 3.14f / locationCount;
		for (int i = 0; i < locationCount; i++)
		{
			float rando = Random.Range(0f, 1f);
			if (rando > 0.9f)
				SpawnRandomFrom(rareLocations, concentricRange);
			else if (rando > 0.6f)
				SpawnRandomFrom(uniqueLocations, concentricRange);
			else
				SpawnRandomFrom(commonLocations, concentricRange);
			concentricRange += (3.14f / locationCount);
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
		while (messy)
		{
			pos = Random.insideUnitSphere * spread * 3;
			pos.y *= 0.1f;
			int numLocations = allLocations.Count;
			bool hitAny = false;
			for (int i = 0; locationCount < numLocations; i++)
			{
				Vector3 toPos = allLocations[i].transform.position - pos;
				if (toPos.magnitude < minDistanceBetweenLocations)
					hitAny = true;
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
				Debug.Log("Destination is unreachable");
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
		float closestDistance = Mathf.Infinity;
		List<CampaignLocation> neighbors = new List<CampaignLocation>(start.GetNeighbors());
		int numNeighbors = neighbors.Count;
		CampaignLocation closestLocation = null;
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
