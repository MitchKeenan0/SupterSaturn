﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetAgent : MonoBehaviour
{
	public int maxCapacity = 100;

	private Game game;
	private Campaign campaign;
	private ObjectManager objectManager;
	private Fleet fleet;
	private Identity identity;
	private FleetController myFleetController;
	private FleetController playerFleetController;
	private LocationManager locationManager;
	private TurnManager turnManager;
	private NameLibrary nameLibrary;
	private List<Spacecraft> spacecraftList;

	public void SetPlayerFleetController(FleetController value) { playerFleetController = value; }
	public List<Spacecraft> GetAgentSpacecraftList() { return spacecraftList; }

	private IEnumerator loadWaitCoroutine;
	private IEnumerator turnWaitCoroutine;

    void Awake()
    {
		spacecraftList = new List<Spacecraft>();
		game = FindObjectOfType<Game>();
		campaign = FindObjectOfType<Campaign>();
		objectManager = FindObjectOfType<ObjectManager>();
		fleet = GetComponentInParent<Fleet>();
		identity = GetComponent<Identity>();
		locationManager = FindObjectOfType<LocationManager>();
		turnManager = FindObjectOfType<TurnManager>();
		myFleetController = transform.parent.GetComponentInChildren<FleetController>();
		nameLibrary = FindObjectOfType<NameLibrary>();

		loadWaitCoroutine = LoadWait(0.2f);
		StartCoroutine(loadWaitCoroutine);
    }

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (fleet != null)
		{
			InitAgent();
			turnWaitCoroutine = TurnWait(waitTime);
			StartCoroutine(turnWaitCoroutine);
		}
	}

	private IEnumerator TurnWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		TakeTurnActions();
	}

	public void TakeTurnActions()
	{
		if ((playerFleetController != null) && (fleet != null))
		{
			if (myFleetController == null)
				myFleetController = transform.parent.GetComponentInChildren<FleetController>();

			if (fleet.GetLocation() != null)
			{
				int neighborIndex = Random.Range(0, fleet.GetLocation().GetNeighbors().Count);
				CampaignLocation neighbor = fleet.GetLocation().GetNeighbors()[neighborIndex];
				myFleetController.StandbyMove(neighbor);
			}
		}
	}

	void InitAgent()
	{
		if (nameLibrary != null)
			fleet.fleetName = nameLibrary.GetFleetName();
		int max = game.GetSpacecraftList().Count * 3;
		int fleetSpacecraftCount = Random.Range(1, max);
		for (int i = 0; i < fleetSpacecraftCount; i++)
		{
			int numCards = game.npcCardLibrary.Length;
			int randomIndex = Random.Range(0, numCards);
			if (randomIndex < numCards)
			{
				GameObject spacecraftObj = Instantiate(game.cardLibrary[randomIndex].cardObjectPrefab, transform);
				Spacecraft sp = spacecraftObj.GetComponent<Spacecraft>();
				spacecraftList.Add(sp);
				game.SetEnemyCard(i, game.cardLibrary[randomIndex]);
				game.AddEnemy(sp);
			}
		}
	}
}
