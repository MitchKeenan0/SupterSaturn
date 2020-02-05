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
	public GameObject[] planetPrefabs;
	public GameObject[] moonPrefabs;
	public GameObject directionalLightPrefab;

	void Start()
    {
        
    }

	public override void InitScene()
	{
		base.InitScene();

		bool solarCenter = false;
		if (Random.Range(0f, 1f) > 0.3f)
		{
			GameObject sunObj = Instantiate(sunPrefab, transform);
			solarCenter = true;
		}

		GameObject backupCenter = null;
		int celestialBodyCount = Random.Range(minimumObjectCount, maximumObjectCount);
		for (int i = 0; i < celestialBodyCount; i++)
		{
			int randomLibraryIndex = Random.Range(0, planetPrefabs.Length - 1);
			GameObject spawnedPlanet = Instantiate(planetPrefabs[randomLibraryIndex], transform);
			if (spawnedPlanet.GetComponent<PlanetArm>())
				spawnedPlanet.GetComponent<PlanetArm>().SetLength((i + 1) * distanceScale);

			int numMoons = Random.Range(0, 10);
			if (numMoons > 0)
			{
				for (int j = 0; j < numMoons; j++)
				{
					int randomMoonIndex = Random.Range(0, moonPrefabs.Length - 1);
					GameObject spawnedMoon = Instantiate(moonPrefabs[randomMoonIndex], spawnedPlanet.transform);
					if (spawnedMoon.GetComponent<PlanetArm>())
						spawnedMoon.GetComponent<PlanetArm>().SetLength((j + 1) * distanceScale);
					i++;
				}
			}

			if (i == 0)
				backupCenter = spawnedPlanet;
		}

		if (!solarCenter)
		{
			GameObject dirLight = Instantiate(directionalLightPrefab, transform);
			dirLight.transform.rotation = Random.rotation;

			if (backupCenter != null)
				backupCenter.transform.position = Vector3.zero;
		}
	}
}
