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
		for (int i = 0; i < locationCount; i++)
		{
			float rando = Random.Range(0f, 1f);
			if (rando > 0.9f)
				SpawnRandomFrom(rareLocations);
			else if (rando > 0.6f)
				SpawnRandomFrom(uniqueLocations);
			else
				SpawnRandomFrom(commonLocations);
		}
	}

	void SpawnRandomFrom(GameObject[] array)
	{
		int index = Random.Range(0, array.Length);
		GameObject prefab = array[index];
		GameObject g = Instantiate(prefab, RandomPositionOnDisk(), Quaternion.identity);
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
		route = new List<CampaignLocation>();
		int numLocations = allLocations.Count;
		for (int i = 0; i < numLocations; i++)
		{
			CampaignLocation thisLocation = allLocations[i];
			thisLocation.SetMarked(false);
			thisLocation.SetDistance(Mathf.Infinity);
			thisLocation.SetPreviousLocation(null);
		}
		start.SetDistance(0f);

		List<CampaignLocation> unvisitedLocations = new List<CampaignLocation>(allLocations);
		CampaignLocation current = start;
		while (unvisitedLocations.Count > 0)
		{
			CampaignLocation nextLocation = null;
			int numNeighbors = current.GetNeighbors().Count;
			if (numNeighbors > 0)
				nextLocation = GetClosestNeighbor(current, destination);
			else
				break;

			current.SetMarked(true);
			if (current == destination)
				break;
			
			unvisitedLocations.Remove(current);

			if (nextLocation != null)
				current = nextLocation;
			else
				break;
		}

		if (destination.IsMarked())
		{
			Stack<CampaignLocation> locationStack = new Stack<CampaignLocation>();
			CampaignLocation index = current;
			locationStack.Push(index);
			while(index != start)
			{
				index = index.PreviousLocation();
				locationStack.Push(index);
			}

			route.Clear();
			int numLocales = locationStack.Count;
			for(int i = 0; i < numLocales; i++)
			{
				route.Add(locationStack.Pop());
			}
		}
		else
		{
			Debug.Log("Destination " + destination.locationName + " was not reached");
		}

		return route;
	}

	CampaignLocation GetClosestNeighbor(CampaignLocation start, CampaignLocation destination)
	{
		CampaignLocation closestLocation = null;
		List<CampaignLocation> neighbors = start.GetNeighbors();
		int numNeighbors = neighbors.Count;
		if (numNeighbors > 0)
		{
			float closestDistance = Mathf.Infinity;
			for(int i = 0; i < numNeighbors; i++)
			{
				CampaignLocation thisNeighbor = neighbors[i];
				if ((thisNeighbor != start) && !thisNeighbor.IsMarked())
				{
					if (thisNeighbor == destination)
						return thisNeighbor;

					thisNeighbor.SetPreviousLocation(start);
					float neighborLocalDistance = Vector3.Distance(thisNeighbor.transform.position, start.transform.position);
					float neighborTotalDistance = neighborLocalDistance + thisNeighbor.PreviousLocation().GetDistance();
					thisNeighbor.SetDistance(neighborTotalDistance);

					if (neighborTotalDistance < closestDistance)
					{
						closestDistance = neighborTotalDistance;
						closestLocation = thisNeighbor;
					}
				}
			}

			// updating 'distant' neighbors
			foreach (CampaignLocation neighbor in neighbors)
			{
				if ((neighbor != start) && (neighbor != closestLocation))
				{
					if (neighbor.GetDistance() > closestDistance)
					{
						neighbor.SetDistance(closestDistance);
						neighbor.SetPreviousLocation(closestLocation);
					}
				}
			}
		}

		return closestLocation;
	}

	CampaignLocation GetNextNeighbor(CampaignLocation value)
	{
		CampaignLocation neighbor = null;
		List<CampaignLocation> neighbors = new List<CampaignLocation>();
		neighbors = value.GetNeighbors();
		foreach(CampaignLocation cl in neighbors)
		{
			if (!cl.IsMarked())
			{
				neighbor = cl;
				break;
			}
		}
		return neighbor;
	}
}
