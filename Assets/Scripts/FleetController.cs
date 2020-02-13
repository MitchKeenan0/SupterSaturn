using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetController : MonoBehaviour
{
	public float fleetMoveSpeed = 1.5f;
	public float movementUpdateInterval = 0.02f;
	public LineRenderer scoutingLine;
	public LineRenderer movementLine;

	private Fleet myFleet;
	private LocationManager locationManager;
	private TurnManager turnManager;
	private Scout scout;
	private CampaignLocation targetLocation;
	private CampaignLocation moveLocation;
	private Campaign campaign;
	private CampaignBattle campaignBattle;
	private Identity identity;
	private List<CampaignLocation> route;
	private List<Fleet> allFleetsList;

	private CampaignLocation scoutLocation;
	private bool bTurnSubmitted = false;
	public bool TurnSubmitted() { return bTurnSubmitted; }
	public void SetTurnSubmitted(bool value) { bTurnSubmitted = value; }

	public int GetTeamID() { return (myFleet != null) ? myFleet.teamID : 0; }

	public CampaignLocation GetLocation() { return myFleet.GetLocation(); }
	public List<CampaignLocation> GetRoute() { return route; }

	private IEnumerator fleetMoveCoroutine;
	private bool bMoving = false;
	public bool IsMoving() { return bMoving; }

	void Awake()
	{
		route = new List<CampaignLocation>();
		allFleetsList = new List<Fleet>();
		myFleet = GetComponentInParent<Fleet>();
		identity = transform.parent.GetComponentInChildren<Identity>();
	}

	void Start()
    {
		Fleet[] allFleets = FindObjectsOfType<Fleet>();
		foreach(Fleet f in allFleets)
		{
			if (f != myFleet)
				allFleetsList.Add(f);
		}

		campaign = FindObjectOfType<Campaign>();
		campaignBattle = FindObjectOfType<CampaignBattle>();
		locationManager = FindObjectOfType<LocationManager>();
		turnManager = FindObjectOfType<TurnManager>();
		scout = GetComponent<Scout>();

		movementLine.enabled = false;
		scoutingLine.enabled = false;

		if (identity != null)
			movementLine.startColor = movementLine.endColor = identity.identityColor;
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
		SetTargetLocation(location);
		CreateRouteTo(location);
		if ((route != null) && (route.Count > 0))
			reachable = true;
		return reachable;
	}

	public void StandbyScout(CampaignLocation location)
	{
		scoutLocation = location;
		scout.Standby(myFleet.GetLocation(), scoutLocation);
	}

	public void ScoutComplete()
	{
		scoutLocation = null;
		bMoving = false;
	}

	public void ExecuteTurn()
	{
		if ((route != null) && (route.Count > 1))
		{
			moveLocation = myFleet.GetRoute()[0];
			fleetMoveCoroutine = FleetMove(movementUpdateInterval);
			StartCoroutine(fleetMoveCoroutine);
			bMoving = true;
		}
		else if (scoutLocation != null)
		{
			scout.ScoutLocation(myFleet.GetLocation(), scoutLocation);
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
		while (myFleet.GetLocation() != moveLocation)
		{
			UpdateMove(updateInterval);
			yield return new WaitForSeconds(updateInterval);
		}
	}

	void CreateRouteTo(CampaignLocation location)
	{
		if (!locationManager)
			locationManager = FindObjectOfType<LocationManager>();
		route.Clear();
		route = locationManager.GetRouteTo(myFleet.GetLocation(), targetLocation);
		if (route != null)
		{
			movementLine.positionCount = route.Count;
			int routeSteps = route.Count;
			if (routeSteps > 0)
			{
				for (int i = 0; i < routeSteps; i++)
				{
					if (route[i] != null)
						movementLine.SetPosition(i, route[i].transform.position);
				}
				movementLine.enabled = true;
				myFleet.SetRoute(route);
			}
		}
	}

	void UpdateMove(float delta)
	{
		Vector3 toMoveLocation = moveLocation.transform.position - myFleet.transform.position;
		if (toMoveLocation.magnitude > 0.05f)
		{
			Vector3 move = myFleet.transform.position + (toMoveLocation.normalized * fleetMoveSpeed * delta);
			myFleet.SetPosition(move);
		}
		else
		{
			FinishMove();
		}
	}

	void FinishMove()
	{
		bMoving = false;

		myFleet.SetLocation(moveLocation, true);

		if (targetLocation != null)
			SetTargetLocation(targetLocation);

		// check for battle
		foreach(Fleet f in allFleetsList)
		{
			CampaignLocation thisLocation = myFleet.GetLocation();
			if ((f.GetLocation() == thisLocation) && (f.teamID != myFleet.teamID))
			{
				if ((myFleet.teamID == 0) || (f.teamID == 0))
				{
					Fleet[] parties = new Fleet[] { myFleet, f };
					campaignBattle.HeraldBattle(thisLocation, parties);
					break;
				}
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
