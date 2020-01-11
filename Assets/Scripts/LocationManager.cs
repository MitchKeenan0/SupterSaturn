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

    void Start()
    {
		InitLocations();
    }

	void InitLocations()
	{
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
		GameObject prefab = array[Mathf.FloorToInt(Random.Range(0f, commonLocations.Length))];
		GameObject g = Instantiate(prefab, RandomPositionOnDisk(), Quaternion.identity);
	}

	Vector3 RandomPositionOnDisk()
	{
		Vector3 pos = Random.insideUnitSphere * spread;
		pos.y = 0f;
		return pos;
	}
}
