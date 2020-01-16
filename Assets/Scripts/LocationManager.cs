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
		route.Add(start);

		CampaignLocation current = start;
		while (destination != current)
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
		}

		return route;
	}

	CampaignLocation GetNextNeighbor(CampaignLocation start, CampaignLocation destination)
	{
		CampaignLocation closestLocation = null;
		List<CampaignLocation> neighbors = new List<CampaignLocation>(start.GetNeighbors());
		float closestDistance = Mathf.Infinity;
		foreach(CampaignLocation cl in neighbors)
		{
			float distanceToDestination = Vector3.Distance(cl.transform.position, destination.transform.position);
			if (distanceToDestination < closestDistance)
			{
				closestDistance = distanceToDestination;
				closestLocation = cl;
			}
		}
		return closestLocation;
	}
}
