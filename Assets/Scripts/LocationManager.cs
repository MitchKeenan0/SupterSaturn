using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
	public float spread = 30f;
	public int locationCount = 12;
	public GameObject[] commonLocations;
	public GameObject[] uniqueLocations; 
	public GameObject[] rareLocations;

	private Campaign campaign;
	private List<CampaignLocation> allLocations;
	private List<CampaignLocation> route;

    void Start()
    {
		route = new List<CampaignLocation>();
		campaign = GetComponent<Campaign>();
		InitLocations();
		campaign.InitFleetLocations(allLocations);
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
		Vector3 pos = Random.insideUnitSphere * spread;
		pos.y *= 0.16f;
		return pos;
	}

	public List<CampaignLocation> GetRouteTo(CampaignLocation start, CampaignLocation destination)
	{
		route.Clear();
		route.Add(start);
		CampaignLocation current = start;
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
			if (safeIndex >= (current.GetNeighbors().Count))
				break;
		}

		return route;
	}

	CampaignLocation GetNextNeighbor(CampaignLocation start, CampaignLocation destination)
	{
		CampaignLocation closestLocation = null;
		List<CampaignLocation> neighbors = new List<CampaignLocation>(start.GetNeighbors());
		float closestDistance = Mathf.Infinity;
		int numNeighbors = neighbors.Count;
		for (int i = 0; i < numNeighbors; i++)
		{
			CampaignLocation cl = neighbors[i];
			if (cl == destination)
				return cl;

			if (cl != start)
			{
				float distanceToDestination = Vector3.Distance(cl.transform.position, destination.transform.position);
				if (distanceToDestination < closestDistance)
				{
					closestDistance = distanceToDestination;
					closestLocation = cl;
				}
			}
		}

		// backup case to explore all remaining neighbors
		if (closestLocation == null)
		{
			for (int i = 0; i < numNeighbors; i++)
			{
				CampaignLocation thisNeighbor = neighbors[i];
				CampaignLocation stepForward = GetNextNeighbor(thisNeighbor, destination);
				if ((stepForward != null) && (stepForward != start))
				{
					if (stepForward == destination)
						return start;

					closestLocation = stepForward;
					break;
				}
			}
		}

		return closestLocation;
	}
}
