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
	private bool bFaceVelocity = false;
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
		if (bEngineActive)
		{
			StopCoroutine(engineCoroutine);
			bEngineActive = false;
			FaceVelocity(true);
		}
		else
		{
			bStopping = true;
			FaceVelocity(false);
			StopAllCoroutines();
		}
		
		spacecraft.MainEngines(0f);
		spacecraft.Maneuver(Vector3.zero);
		spacecraft.SideJets(Vector3.zero);
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
		if (bEngineActive)
			StopCoroutine(engineCoroutine);
		if (bAligning)
			StopCoroutine(alignCoroutine);
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

	public void FaceVelocity(bool value)
	{
		bFaceVelocity = value;
	}

	private IEnumerator alignCoroutine;
	IEnumerator Align(float intervalTime)
	{
		bAligning = true;
		float dot = 0f;
		bool burn = false;
		while (dot < 0.99f)
		{
			ManeuverRotationTo(destination - rb.velocity);

			Vector3 alignVector = ((destination - rb.velocity) - spacecraft.transform.position);
			Vector3 myVelocity = rb.velocity;
			Vector3 projectedVelocity = Vector3.ProjectOnPlane(myVelocity, spacecraft.transform.forward.normalized);
			projectedVelocity.z = Mathf.Clamp(projectedVelocity.z, -1f, 0f);
			spacecraft.SideJets(projectedVelocity * -1f);

			Vector3 myVector = rb.velocity;
			if (myVector.magnitude < 1f)
				myVector = transform.forward;
			dot = Vector3.Dot(myVector.normalized, alignVector.normalized);

			if (!burn && (dot > 0.7f))
			{
				burn = true;
				engineCoroutine = MainEngineBurn(burnDuration);
				StartCoroutine(engineCoroutine);
			}

			yield return new WaitForSeconds(intervalTime);
		}

		spacecraft.SideJets(Vector3.zero);
		bAligning = false;
	}

	IEnumerator MainEngineBurn(float durationTime)
	{
		spacecraft.MainEngines(1f);
		float timeElapsed = 0f;
		bEngineActive = true;
		if (rb.velocity.magnitude < 1f)
			FindObjectOfType<InputController>().AllStop(false);
		while (timeElapsed < durationTime)
		{
			timeElapsed += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		spacecraft.MainEngines(0f);
		FaceVelocity(true);
		bEngineActive = false;
	}
}
