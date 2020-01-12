using System.Collections;
using System.Collections.Generic;
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
		pos.y = 0f;
		return pos;
	}
}
