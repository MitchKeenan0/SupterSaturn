using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NebulaSystem : BattleScene
{
	public GameObject[] cloudLibrary;
	public GameObject[] starLibrary;
	public int minimumObjectCount = 10;
	public int maximumObjectCount = 30;
	public float distanceScale = 3;
	public float cloudMinSize = 1;
	public float cloudMaxSize = 10;
	public float starMinSize = 0.1f;
	public float starMaxSize = 1;

    void Start()
    {
        
    }

	public override void InitScene()
	{
		base.InitScene();

		int nebulaObjectCount = Random.Range(minimumObjectCount, maximumObjectCount);
		for(int i = 0; i < nebulaObjectCount; i++)
		{
			GameObject[] library = cloudLibrary;
			bool bCloud = true;
			float biasedCoinToss = Random.Range(0f, 1f);
			if (biasedCoinToss < 0.3f)
			{
				library = starLibrary;
				bCloud = false;
			}

			int prefabIndex = Random.Range(0, library.Length - 1);
			GameObject spawnObj = Instantiate(library[prefabIndex], transform);
			Vector3 spawnScale = Vector3.one * Random.Range(cloudMinSize, cloudMaxSize);
			if (!bCloud)
				spawnScale = Vector3.one * Random.Range(starMinSize, starMaxSize);
			spawnObj.transform.localScale = spawnScale;

			Vector3 spawnPosition = Random.insideUnitCircle * i * distanceScale;
			spawnObj.transform.position = spawnPosition;
		}
	}
}
