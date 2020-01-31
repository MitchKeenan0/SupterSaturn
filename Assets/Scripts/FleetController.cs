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

	public CampaignLocation GetLocation() { return fleet.GetLocation(); }
	public List<CampaignLocation> GetRoute() { return route; }

	private IEnumerator fleetMoveCoroutine;
	private bool bMoving = false;
	public bool IsMoving() { return bMoving; }

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
		if (location == null)
			moveLocation = null;
	}

	public bool StandbyMove(CampaignLocation location)
	{
		bool reachable = false;
		route = new List<CampaignLocation>();
		SetTargetLocation(location);
		CreateRouteTo(location);
		if ((route != null) && (route.Count > 0))
			reachable = true;
		Debug.Log(transform.name + " standby move to " + location.locationName);
		return reachable;
	}

	public void StandbyScout(CampaignLocation location)
	{
		scoutLocation = location;
		scout.Standby(fleet.GetLocation(), scoutLocation);
	}

	public void ScoutComplete()
	{
		scoutLocation = null;
		bMoving = false;
	}

	public void ExecuteTurn()
	{
		if (scoutLocation != null)
		{
			scout.ScoutLocation(fleet.GetLocation(), scoutLocation);
			bMoving = true;
		}

		if ((route != null) && (route.Count > 1))
		{
			moveLocation = route[1];
			fleetMoveCoroutine = FleetMove(movementUpdateInterval);
			StartCoroutine(fleetMoveCoroutine);
			bMoving = true;
		}
	}

	public void Undo()
	{
		SetTargetLocation(null);
		if (route != null)
			route.Clear();
		scout.Undo();
		scoutLocation = null;
		ClearLines();
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
		if (route != null)
		{
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
	}

	void UpdateMove(float delta)
	{
		Vector3 toMoveLocation = moveLocation.transform.position - fleet.transform.position;
		if (toMoveLocation.magnitude > 0.05f)
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
		bMoving = false;
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

		ClearLines();

		// reset turn manager
		if (GetTeamID() == 0)
			turnManager.BeginTurn(route.Count > 2);
		//StandbyMove(targetLocation);
	}

	void ClearLines()
	{
		movementLine.enabled = false;
		scoutingLine.enabled = false;
	}
}
