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

    void Start()
    {
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
		List<CampaignLocation> route = new List<CampaignLocation>();
		int numLocations = allLocations.Count;
		for (int i = 0; i < numLocations; i++)
		{
			CampaignLocation thisLocation = allLocations[i];
			thisLocation.SetMarked(false);
		}
		CampaignLocation current = start;
		Debug.Log("Starting at " + current.locationName);
		for (int i = 0; i < numLocations; i++)
		{
			if (current != null)
			{
				current.SetMarked(true);
				route.Add(current);
				if (current == destination)
					break;
				CampaignLocation next = GetClosestLocation(current, destination);
				current = next;
			}
		}
		return route;
	}

	CampaignLocation GetClosestLocation(CampaignLocation start, CampaignLocation end)
	{
		CampaignLocation location = null;
		float closestDistance = Mathf.Infinity;

		int numLocations = allLocations.Count;
		for (int i = 0; i < numLocations; i++)
		{
			CampaignLocation cl = allLocations[i];
			if ((cl != start) && !cl.IsMarked())
			{
				float distance = Vector3.Distance(cl.transform.position, start.transform.position);
				if ((distance < closestDistance))
				{
					closestDistance = distance;
					location = cl;
				}
			}
		}

		if (start != null && location != null)
			Debug.Log("Closest to " + start.locationName + " is " + location.locationName);

		return location;
	}
}
