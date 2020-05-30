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
			alignCoroutine = Align(Time.deltaTime);
			StartCoroutine(alignCoroutine);
		}
		else
		{
			engineCoroutine = MainEngineBurn(burnDuration);
			StartCoroutine(engineCoroutine);
		}
	}

	private IEnumerator alignCoroutine;
	IEnumerator Align(float intervalTime)
	{
		bAligning = true;
		float dot = 0f;
		bool burn = false;
		while (!burn)
		{
			ManeuverRotationTo(destination);

			yield return new WaitForSeconds(intervalTime);

			Vector3 alignVector = (destination - spacecraft.transform.position);
			Vector3 myVector = rb.velocity;
			if (myVector.magnitude < 1f)
				myVector = transform.forward;
			dot = Vector3.Dot(myVector.normalized, alignVector.normalized);
			if (dot > 0.7f)
			{
				engineCoroutine = MainEngineBurn(burnDuration);
				StartCoroutine(engineCoroutine);
				burn = true;
			}
		}
		
		bAligning = false;
	}

	IEnumerator MainEngineBurn(float durationTime)
	{
		spacecraft.MainEngines(1f);
		bEngineActive = true;
		//if (rb.velocity.magnitude < 1f)
		//	FindObjectOfType<InputController>().AllStop(false);
		ReleaseBrakes();

		yield return new WaitForSeconds(durationTime);

		spacecraft.MainEngines(0f);
		bEngineActive = false;
	}

	public void MainEngineShutdown()
	{
		spacecraft.MainEngines(0f);
		bEngineActive = false;
		StopAllCoroutines();
	}
}
