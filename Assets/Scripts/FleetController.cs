using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetController : MonoBehaviour
{
	public float fleetMoveSpeed = 1.5f;
	public float movementUpdateInterval = 0.02f;
	public LineRenderer scoutingLine;
	public LineRenderer movementLine;

	private Fleet fleet;
	private LocationManager locationManager;
	private TurnManager turnManager;
	private Scout scout;
	private CampaignLocation targetLocation;
	private CampaignLocation moveLocation;
	private Campaign campaign;
	private CampaignBattle campaignBattle;
	private List<CampaignLocation> route;
	private List<Fleet> allFleetsList;

	private CampaignLocation scoutLocation;
	private bool bTurnSubmitted = false;
	public bool TurnSubmitted() { return bTurnSubmitted; }
	public void SetTurnSubmitted(bool value) { bTurnSubmitted = value; }

	public int GetTeamID() { return (fleet != null) ? fleet.teamID : 0; }

	private IEnumerator fleetMoveCoroutine;

	void Awake()
	{
		route = new List<CampaignLocation>();
		allFleetsList = new List<Fleet>();
		fleet = GetComponentInParent<Fleet>();
	}

	void Start()
    {
		Fleet[] allFleets = FindObjectsOfType<Fleet>();
		foreach(Fleet f in allFleets)
		{
			if (f != fleet)
				allFleetsList.Add(f);
		}

		campaign = FindObjectOfType<Campaign>();
		campaignBattle = FindObjectOfType<CampaignBattle>();
		locationManager = FindObjectOfType<LocationManager>();
		turnManager = FindObjectOfType<TurnManager>();
		scout = GetComponent<Scout>();

		movementLine.enabled = false;
		scoutingLine.enabled = false;
	}

	public void SetTargetLocation(CampaignLocation location)
	{
		targetLocation = location;
		if (targetLocation == null)
		{
			moveLocation = null;
			Debug.Log("Set move location null");
		}
	}

	public void StandbyMove(CampaignLocation location)
	{
		SetTargetLocation(location);
		CreateRouteTo(location);
	}

	public void StandbyScout(CampaignLocation location)
	{
		scoutLocation = location;
		scout.Standby(fleet.GetLocation(), scoutLocation);
	}

	public void ScoutComplete()
	{
		scoutLocation = null;
	}

	public void ExecuteTurn()
	{
		if (scoutLocation != null)
		{
			scout.ScoutLocation(fleet.GetLocation(), scoutLocation);
		}

		if (route.Count > 1)
		{
			moveLocation = route[1];
			fleetMoveCoroutine = FleetMove(movementUpdateInterval);
			StartCoroutine(fleetMoveCoroutine);
		}
	}

	public void Undo()
	{
		SetTargetLocation(null);
		route.Clear();
		scout.Undo();
		scoutLocation = null;
		ClearLines();
		Debug.Log("Undo");
	}
	
	private IEnumerator FleetMove(float updateInterval)
	{
		while (fleet.GetLocation() != moveLocation)
		{
			UpdateMove(updateInterval);
			yield return new WaitForSeconds(updateInterval);
		}
	}

	void CreateRouteTo(CampaignLocation location)
	{
		if (!locationManager)
			locationManager = FindObjectOfType<LocationManager>();
		route = locationManager.GetRouteTo(fleet.GetLocation(), targetLocation);

		movementLine.positionCount = 0;
		movementLine.positionCount = route.Count;
		int routeSteps = route.Count;
		for (int i = 0; i < routeSteps; i++)
		{
			if (route[i] != null)
				movementLine.SetPosition(i, route[i].transform.position);
		}

		movementLine.enabled = true;
	}

	void UpdateMove(float delta)
	{
		Vector3 toMoveLocation = moveLocation.transform.position - fleet.transform.position;
		if (toMoveLocation.magnitude > 0.1f)
		{
			Vector3 move = fleet.transform.position + (toMoveLocation.normalized * fleetMoveSpeed * delta);
			fleet.SetPosition(move);
		}
		else
		{
			FinishMove();
		}
	}

	void FinishMove()
	{
		fleet.SetLocation(moveLocation, true);
		if (targetLocation != null)
			SetTargetLocation(targetLocation);

		// check for battle
		foreach(Fleet f in allFleetsList)
		{
			CampaignLocation thisLocation = fleet.GetLocation();
			if ((f.GetLocation() == thisLocation) && f.teamID != fleet.teamID)
			{
				Fleet[] parties = new Fleet[] { fleet, f };
				campaignBattle.HeraldBattle(thisLocation, parties);
			}
		}

		// reset turn manager
		turnManager.BeginTurn(route.Count > 2);
		StandbyMove(targetLocation);
	}

	void ClearLines()
	{
		movementLine.enabled = false;
		scoutingLine.enabled = false;
	}
}
