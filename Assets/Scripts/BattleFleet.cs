﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFleet : MonoBehaviour
{
	public int teamID = 0;
	public Vector3[] groupPositions;

	private Game game;
	private Player player;

	private IEnumerator loadWaitCoroutine;
    
	void Awake()
	{
		game = FindObjectOfType<Game>();
		player = FindObjectOfType<Player>();
	}

    void Start()
    {
		loadWaitCoroutine = LoadWait(0.1f);
		StartCoroutine(loadWaitCoroutine);
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (teamID == 0)
			InitSavedFleet();
	}

	void InitSavedFleet()
	{
		List<Spacecraft> spacecraftList = new List<Spacecraft>(game.GetSpacecraftList());
		int numCards = spacecraftList.Count;
		for (int i = 0; i < numCards; i++)
		{
			Spacecraft sp = spacecraftList[i];
			Vector3 amidstPosition = Vector3.forward * -i;
			if (groupPositions.Length > i)
				amidstPosition = groupPositions[i];
			sp.transform.position = transform.position + amidstPosition;
			sp.transform.SetParent(transform);
			sp.gameObject.SetActive(true);
		}
	}
}