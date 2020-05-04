using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointHud : MonoBehaviour
{
	public Transform checkpointListPanel;
	public GameObject checkpointListingPrefab;

	private List<CelestialBody> celestialBodyList;
	private List<CheckpointListing> checkpointListingList;
	private IEnumerator loadCoroutine;

    void Start()
    {
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
			CelestialBody cb = celestials[i];
			celestialBodyList.Add(cb);
			CheckpointListing cl = SpawnListing();
			string checkpointName = cb.celestialBodyName;
			cl.SetListing(checkpointName);
			checkpointListingList.Add(cl);
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
		int bodyIndex = celestialBodyList.IndexOf(celestialBody);
		CheckpointListing cl = checkpointListingList[bodyIndex];
		cl.ClearCheckpoint();
	}
}
