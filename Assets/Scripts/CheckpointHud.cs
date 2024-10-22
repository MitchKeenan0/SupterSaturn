﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointHud : MonoBehaviour
{
	public Transform checkpointListPanel;
	public GameObject checkpointListingPrefab;
	public Color[] colorPool;

	private List<CelestialBody> celestialBodyList;
	private List<CheckpointListing> checkpointListingList;
	private IEnumerator loadCoroutine;

	private BattleOutcome outcome = null;
	private Timer timer = null;
	private int checkpoints = 0;
	private int clearedCheckpoints = 0;

    void Start()
    {
		outcome = FindObjectOfType<BattleOutcome>();
		timer = FindObjectOfType<Timer>();
		loadCoroutine = LoadDelay(0.3f);
		StartCoroutine(loadCoroutine);
    }

	void InitCheckpoints()
	{
		celestialBodyList = new List<CelestialBody>();
		checkpointListingList = new List<CheckpointListing>();
		CelestialBody[] celestials = FindObjectsOfType<CelestialBody>();
		int num = celestials.Length;
		for(int i = 0; i < num; i++)
		{
			CelestialBody celestialBody = celestials[i];
			celestialBodyList.Add(celestialBody);

			int randomColorIndex = Random.Range(0, colorPool.Length);
			Color checkpointColor = colorPool[randomColorIndex];
			Checkpoint checkpoint = celestialBody.gameObject.GetComponentInChildren<Checkpoint>();
			if (checkpoint != null)
			{
				checkpoints++;
				checkpoint.SetColor(checkpointColor);
				string checkpointName = celestialBody.celestialBodyName;
				CheckpointListing checkpointListing = SpawnListing();
				checkpointListing.SetListing(checkpointName, checkpointColor, celestialBody.GetComponentInChildren<MeshRenderer>().transform);
				checkpointListingList.Add(checkpointListing);
			}
		}
	}

	CheckpointListing SpawnListing()
	{
		GameObject listingObj = Instantiate(checkpointListingPrefab, checkpointListPanel);
		CheckpointListing cl = listingObj.GetComponent<CheckpointListing>();
		return cl;
	}

	private IEnumerator LoadDelay(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitCheckpoints();
	}

	public void CheckpointClear(CelestialBody celestialBody)
	{
		if (celestialBodyList == null)
			celestialBodyList = new List<CelestialBody>();
		int bodyIndex = celestialBodyList.IndexOf(celestialBody) - 1;
		if (checkpointListingList[bodyIndex] != null)
		{
			CheckpointListing checkpointListing = checkpointListingList[bodyIndex];
			checkpointListing.ClearCheckpoint();
			clearedCheckpoints++;
			if (clearedCheckpoints >= checkpoints)
			{
				float time = Time.timeSinceLevelLoad;
				int objValue = FindObjectOfType<ObjectiveType>().objectiveValue;
				float levelScore = objValue / time;
				FindObjectOfType<ScoreHUD>().UpdateScore(Mathf.RoundToInt(levelScore));
				if (outcome != null)
				{
					outcome.BattleOver(true);
					Checkpoint cp = celestialBody.GetComponentInChildren<Checkpoint>();
					if (cp != null)
					{
						
					}
				}
				timer.Freeze();
			}
		}
	}
}
