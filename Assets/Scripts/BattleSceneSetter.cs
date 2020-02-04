using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneSetter : MonoBehaviour
{
	public GameObject[] celestialBodyLibrary;
	public GameObject[] nebulaLibrary;
	public GameObject[] spaceStructureLibrary;
	public GameObject directionalLightPrefab;
	public int minObjectCount = 0;
	public int maxObjectCount = 25;

	private Game game;
	private IEnumerator loadWaitCoroutine;

    void Start()
    {
		game = FindObjectOfType<Game>();
		loadWaitCoroutine = LoadWait(0.3f);
		StartCoroutine(loadWaitCoroutine);
    }

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		LoadSceneSetting();
	}

	void LoadSceneSetting()
	{
		GameObject[] library = new GameObject[0];
		int sceneType = game.GetSceneSetting();
		if (sceneType == 0)
			library = celestialBodyLibrary;
		else if (sceneType == 1)
			library = nebulaLibrary;
		else if (sceneType == 2)
			library = spaceStructureLibrary;

		Debug.Log("Scene type: " + sceneType);

		if (library.Length >= 1)
		{
			int randomCount = Random.Range(minObjectCount, maxObjectCount);
			for (int i = 0; i < randomCount; i++)
			{
				int randomLibraryIndex = Random.Range(0, library.Length - 1);
				GameObject spawnedObject = Instantiate(library[randomLibraryIndex], transform);
				Vector3 spawnPosition = Random.onUnitSphere * Random.Range(2f, 25f);
				if (sceneType == 0)
					spawnPosition.y *= 0.1f;
				if (spawnedObject.GetComponent<PlanetArm>())
					spawnedObject.GetComponent<PlanetArm>().SetLength(i + 1);
				else if (!spawnedObject.GetComponent<Sun>())
					spawnedObject.transform.position = spawnPosition;
			}
		}

		Sun sun = FindObjectOfType<Sun>();
		if (!sun)
		{
			GameObject dirLight = Instantiate(directionalLightPrefab, transform);
			dirLight.transform.rotation = Random.rotation;
		}
	}
}
