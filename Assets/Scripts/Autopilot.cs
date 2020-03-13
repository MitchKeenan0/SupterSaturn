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
	private bool bExecutingMoveCommand = false;
	private bool bEngineActive = false;
	private bool bFaceVelocity = false;
	private bool bStopping = false;
	private float timeAtLastMoveCommand = 0;
	private float burnDuration = 0f;
	private List<Vector3> routeVectors;
	private Vector3 targetPosition = Vector3.zero;
	private Vector3 destination = Vector3.zero;

	private IEnumerator engineCoroutine;

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
		Debug.DrawLine(transform.position, destination, Color.grey);

		if (bFaceVelocity && (rb.velocity.magnitude > 1f))
		{
			Vector3 velocity = rb.velocity + transform.position;
			ManeuverRotationTo(velocity);
		}

		if (bStopping)
		{
			float velocity = rb.velocity.magnitude;
			if (Mathf.Abs(velocity) > 0f)
				spacecraft.Brake();
		}
	}

	public void AllStop()
	{
		bStopping = true;
		bExecutingMoveCommand = false;
		spacecraft.MainEngines(0f);
		spacecraft.Maneuver(Vector3.zero);
		spacecraft.SideJets(Vector3.zero);
		FaceVelocity(false);
	}

	public void ManeuverRotationTo(Vector3 target)
	{
		destination = target;
		Vector3 toDestination = destination - spacecraft.transform.position;
		spacecraft.Maneuver(toDestination.normalized);
	}
	
	public void FireEngineBurn(float duration)
	{
		if (bEngineActive)
			StopAllCoroutines();
		burnDuration = duration / 2f;
		alignCoroutine = Align(Time.deltaTime);
		StartCoroutine(alignCoroutine);
	}

	public void FaceVelocity(bool value)
	{
		bFaceVelocity = value;
	}

	private IEnumerator alignCoroutine;
	IEnumerator Align(float intervalTime)
	{
		float dot = 0f;
		while (dot < 0.99f)
		{
			ManeuverRotationTo(destination - rb.velocity);

			Vector3 alignVector = (destination - spacecraft.transform.position);
			Vector3 myVelocity = rb.velocity;
			Vector3 projectedVelocity = Vector3.ProjectOnPlane(myVelocity, alignVector.normalized);
			projectedVelocity.z = 0f;
			spacecraft.SideJets(projectedVelocity * -1f);
			
			dot = Vector3.Dot(transform.forward, alignVector.normalized);
			Debug.Log("aligning dot " + dot);

			yield return new WaitForSeconds(intervalTime);
		}
		spacecraft.SideJets(Vector3.zero);
		engineCoroutine = MainEngineBurn(burnDuration);
		StartCoroutine(engineCoroutine);
	}

	IEnumerator MainEngineBurn(float durationTime)
	{
		Debug.Log("main engines ignited for " + durationTime);
		spacecraft.MainEngines(1f);
		float timeElapsed = 0f;
		bEngineActive = true;
		bStopping = false;
		while (timeElapsed < durationTime)
		{
			timeElapsed += Time.deltaTime;
			if ((rb.velocity.magnitude > 5f) &&
				(Vector3.Dot(rb.velocity.normalized, (destination - spacecraft.transform.position).normalized) > 0.8f))
			{
				FaceVelocity(true);
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}
		spacecraft.MainEngines(0f);
		bEngineActive = false;
	}
}
