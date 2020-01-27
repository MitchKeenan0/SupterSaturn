﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet : MonoBehaviour
{
	public int teamID = 0;
	public string fleetName = "My Fleet";
	public int fleetCapacity = 10;

	private Game game;
	private Player player;
	private Campaign campaign;
	private List<Spacecraft> spacecraftList;
	private CampaignLocation campaignLocation;
	private FleetController fleetController;

	private IEnumerator loadWaitCoroutine;

    void Awake()
    {
		spacecraftList = new List<Spacecraft>();
		game = FindObjectOfType<Game>();
		player = FindObjectOfType<Player>();
		campaign = FindObjectOfType<Campaign>();
		fleetController = GetComponentInChildren<FleetController>();
	}

	void Start()
	{
		if (teamID == 0)
		{
			loadWaitCoroutine = LoadWait(0.1f);
			StartCoroutine(loadWaitCoroutine);
		}
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		InitSavedFleet();
		campaign.InitFleetLocations();
	}

	void InitSavedFleet()
	{
		fleetName = player.playerName;
		if (fleetName == "")
			fleetName = "No Name";

		spacecraftList = game.GetSpacecraftList();
	}

	public void SetName(string value)
	{
		fleetName = value;
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}

	public void SetLocation(CampaignLocation location, bool position)
	{
		campaignLocation = location;
		if (position)
			transform.position = location.transform.position;
	}

	public CampaignLocation GetLocation() { return campaignLocation; }

	public List<Spacecraft> GetSpacecraftList()
	{
		return spacecraftList;
	}
}
