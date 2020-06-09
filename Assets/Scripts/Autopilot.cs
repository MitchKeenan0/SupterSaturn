using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Autopilot : MonoBehaviour
{
	public int routeVectorPoints = 10;
	public float updateInterval = 0.2f;
	public float autopilotSkill = 0.9f;
	public float autopilotAreaRange = 20f;
	public bool bRotationFollowVelocity = true;

	private Rigidbody rb;
	private Agent agent;
	private Spacecraft spacecraft;
	private RouteVisualizer routeVisualizer;
	private ObjectManager objectManager;
	private SelectionSquad selectionSquad;
	private Transform followTransform;
	private Vector3 commandMoveVector = Vector3.zero;
	private Vector3 gravityEscapePosition = Vector3.zero;
	private Vector3 targetCombatPosition = Vector3.zero;
	private Vector3 holdPosition = Vector3.zero;
	private Vector3 autoPilotStartPosition = Vector3.zero;
	private Vector3 maneuverVector = Vector3.zero;
	private Vector3 previousRouteVector = Vector3.zero;
	private bool bEngineActive = false;
	private bool bStopping = false;
	private bool bAligning = false;
	private float burnDuration = 0f;
	private List<Vector3> routeVectors;
	private Vector3 targetPosition = Vector3.zero;
	private Vector3 destination = Vector3.zero;

	private IEnumerator engineCoroutine;

	public bool IsEngineActive() { return bEngineActive; }

	void Start()
    {
		rb = GetComponent<Rigidbody>();
		agent = GetComponent<Agent>();
		spacecraft = GetComponent<Spacecraft>();
		routeVisualizer = GetComponentInChildren<RouteVisualizer>();
		objectManager = FindObjectOfType<ObjectManager>();
		selectionSquad = FindObjectOfType<SelectionSquad>();

		routeVectors = new List<Vector3>();
		holdPosition = spacecraft.transform.position;
		targetCombatPosition = transform.position;
		autoPilotStartPosition = transform.position;
		previousRouteVector = transform.position;
	}

	void Update()
	{
		if (bStopping)
			spacecraft.Brake();
		if (bAligning)
			Align();
	}

	public void AllStop()
	{
		bStopping = true;
		bEngineActive = false;
		StopAllCoroutines();

		spacecraft.MainEngines(0f);
		spacecraft.Maneuver(Vector3.zero);
		//spacecraft.SideJets(Vector3.zero);
		spacecraft.SetThrottle(0);

		Debug.Log("All stop ");
	}

	public void ReleaseBrakes()
	{
		bStopping = false;
	}

	public void ManeuverRotationTo(Vector3 target)
	{
		destination = target;
		Vector3 toDestination = destination - spacecraft.transform.position;
		spacecraft.Maneuver(toDestination.normalized);
	}
	
	public void FireEngineBurn(float duration, bool bAlign)
	{
		//StopAllCoroutines();
		burnDuration = duration;
		if (bAlign)
		{
			bAligning = true;
		}
		else
		{
			MainEngineBurn();
		}
	}

	void Align()
	{
		ManeuverRotationTo(destination);

		Vector3 alignVector = (destination - spacecraft.transform.position);
		Vector3 myVector = spacecraft.transform.forward;
		float dot = Vector3.Dot(myVector.normalized, alignVector.normalized);
		if (dot > 0.7f)
		{
			bAligning = false;
			MainEngineBurn();
		}
	}

	void MainEngineBurn()
	{
		spacecraft.MainEngines(1f);
		bEngineActive = true;
		ReleaseBrakes();
	}

	public void MainEngineShutdown()
	{
		spacecraft.MainEngines(0f);
		bEngineActive = false;
		StopAllCoroutines();
	}
}
