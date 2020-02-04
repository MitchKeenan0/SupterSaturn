using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : BattleScene
{
	public int minimumObjectCount = 1;
	public int maximumObjectCount = 25;
	public float distanceScale = 1.6f;
	public float planetMinSize = 0.1f;
	public float planetMaxSize = 1;
	public GameObject sunPrefab;
	public GameObject[] celestialBodyPrefabs;
	public GameObject directionalLightPrefab;

	void Start()
    {
        
    }

	public override void InitScene()
	{
		base.InitScene();

		bool spawnedSun = false;
		if (Random.Range(0f,1f) > 0.3f)
		{
			GameObject sunObj = Instantiate(sunPrefab, transform);
			spawnedSun = true;
		}

		float largestSize = 0;
		GameObject backupCenter = null;
		int celestialBodyCount = Random.Range(minimumObjectCount, maximumObjectCount);
		for (int i = 0; i < celestialBodyCount; i++)
		{
			int randomLibraryIndex = Random.Range(0, celestialBodyPrefabs.Length - 1);
			GameObject spawnedObject = Instantiate(celestialBodyPrefabs[randomLibraryIndex], transform);
			Vector3 spawnPosition = Random.onUnitSphere * Random.Range(2f, 25f);
			spawnPosition.y *= 0.1f;

			if (spawnedObject.GetComponent<PlanetArm>())
				spawnedObject.GetComponent<PlanetArm>().SetLength(i * distanceScale);

			Vector3 planetSize = Vector3.one * Random.Range(planetMinSize, planetMaxSize);
			spawnedObject.transform.localScale = planetSize;
			if (planetSize.magnitude > largestSize)
			{
				largestSize = planetSize.magnitude;
				backupCenter = spawnedObject;
			}
		}

		if (!spawnedSun)
		{
			GameObject dirLight = Instantiate(directionalLightPrefab, transform);
			dirLight.transform.rotation = Random.rotation;

			if (backupCenter != null)
				backupCenter.transform.position = Vector3.zero;
		}
	}
}
