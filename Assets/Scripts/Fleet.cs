using System.Collections;
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
	private FleetAgent fleetAgent;
	private Identity identity;
	private List<Spacecraft> spacecraftList;
	private CampaignLocation campaignLocation;
	private FleetController fleetController;
	private List<CampaignLocation> routeList;

	private IEnumerator loadWaitCoroutine;

    void Awake()
    {
		spacecraftList = new List<Spacecraft>();
		routeList = new List<CampaignLocation>();
		game = FindObjectOfType<Game>();
		player = FindObjectOfType<Player>();
		campaign = FindObjectOfType<Campaign>();
		fleetAgent = GetComponentInChildren<FleetAgent>();
		fleetController = GetComponentInChildren<FleetController>();
	}

	void Start()
	{
		loadWaitCoroutine = LoadWait(0.15f);
		StartCoroutine(loadWaitCoroutine);
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitSavedFleet();
	}

	void InitSavedFleet()
	{
		if (teamID == 0)
		{
			fleetName = player.playerName;
			if (fleetName == "")
				fleetName = "No Name";
			spacecraftList = game.GetSpacecraftList();
		}
		else if (fleetAgent != null)
		{
			spacecraftList = fleetAgent.GetAgentSpacecraftList();
		}
	}

	public void SetIdentity(Identity id)
	{
		identity = id;
		if (GetLocation() != null)
			identity.SetLocation(GetLocation());
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
		if (campaignLocation != null)
			campaignLocation.ConnectionsLit(false);

		campaignLocation = location;
		campaignLocation.ConnectionsLit(teamID == 0);
		if (position)
			transform.position = location.transform.position;

		if (!identity)
			identity = GetComponentInChildren<Identity>();
		if (identity != null)
			identity.SetLocation(location);
	}

	public void SetRoute(List<CampaignLocation> list)
	{
		routeList.Clear();
		if (list != null)
		{
			foreach (CampaignLocation cl in list)
			{
				if (cl != campaignLocation)
					routeList.Add(cl);
			}
		}
	}

	public List<CampaignLocation> GetRoute() { return routeList; }

	public CampaignLocation GetLocation() { return campaignLocation; }

	public List<Spacecraft> GetSpacecraftList()
	{
		return spacecraftList;
	}
}
