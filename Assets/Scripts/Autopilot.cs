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
	private float timeAtLastMoveCommand = 0;
	private List<Vector3> routeVectors;
	private Vector3 targetPosition = Vector3.zero;

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

	public void SpacecraftNavigationCommands()
	{
		if (bExecutingMoveCommand && (routeVectors.Count > 0))
		{
			Vector3 currentRouteVector = routeVectors.ElementAt(0);
			FlyTo(currentRouteVector);

			float distance = Vector3.Distance(transform.position, currentRouteVector);
			if ((distance < 10f) && ((Time.time - timeAtLastMoveCommand) > 1f))
			{
				holdPosition = transform.position;
				routeVectors.Remove(currentRouteVector);
				if (routeVisualizer != null)
					routeVisualizer.ClearLine(0);

				if (routeVectors.Count == 0)
				{
					EnableMoveCommand(false);
					AllStop();
				}
			}
		}
		else
		{
			FlyTo(holdPosition);
		}
	}

	public void SetRoute(List<Vector3> vectors)
	{
		ClearRoute();

		if (routeVectors == null)
			routeVectors = new List<Vector3>();

		if ((vectors != null) && (vectors.Count > 0))
		{
			int numVectors = vectors.Count;
			for(int i = 0; i < numVectors; i++)
			{
				Vector3 routeStep = vectors[i];
				routeVectors.Add(routeStep);
			}

			OptimizeRoute(routeVectors);

			Vector3 previous = Vector3.zero;
			for (int i = 0; i < numVectors; i++)
			{
				Vector3 routeStep = vectors[i];
				if (previous != Vector3.zero)
					routeVisualizer.DrawLine(i, previous, routeStep);
				previous = routeStep;
			}
		}
	}

	public void SetMoveCommand(Vector3 value, bool bOrbital)
	{
		//commandMoveVector = value;
		//if (Vector3.Distance(commandMoveVector, transform.position) > 0.1f)
		//	GenerateRoute(bOrbital);
	}

	public void EnableMoveCommand(bool value)
	{
		bExecutingMoveCommand = value;
		if (!routeVisualizer)
			routeVisualizer = GetComponentInChildren<RouteVisualizer>();
		if (routeVisualizer != null)
		{
			if (bExecutingMoveCommand)
			{
				routeVisualizer.SetRouteColor(Color.grey);
			}
			else if (spacecraft != null)
			{
				routeVisualizer.ClearLine(-1);
				holdPosition = transform.position;
			}
		}

		if (bExecutingMoveCommand)
			timeAtLastMoveCommand = Time.time;
		else
			ClearRoute();
	}

	public void AllStop()
	{
		bExecutingMoveCommand = false;
		spacecraft.MainEngines(0f);
		spacecraft.Maneuver(Vector3.zero);
	}

	public void SetFollowTransform(Transform value)
	{
		followTransform = value;
	}

	void OptimizeRoute(List<Vector3> vectors)
	{
		int numVectors = vectors.Count;
		previousRouteVector = transform.position;
		List<Vector3> optimalRoute = new List<Vector3>();
		List<Vector3> usedPoints = new List<Vector3>();

		// get insertion point
		float bestMatch = 0f;
		Vector3 insertionPoint = Vector3.zero;
		Vector3 previousVector = vectors[0];
		for (int i = 0; i < numVectors; i++)
		{
			Vector3 thisVector = vectors[i] - previousVector;
			Vector3 toSpacecraft = (vectors[i] - spacecraft.transform.position);
			float thisDot = Vector3.Dot(toSpacecraft.normalized, thisVector.normalized);
			if (thisDot > bestMatch)
			{
				bestMatch = thisDot;
				insertionPoint = vectors[i];
			}
			previousVector = vectors[i];
		}
		optimalRoute.Add(insertionPoint);
		usedPoints.Add(insertionPoint);

		// follow through
		while (optimalRoute.Count < vectors.Count)
		{
			int startingIndex = vectors.IndexOf(insertionPoint);
			for (int i = startingIndex; i < numVectors; i++)
			{
				Vector3 nextPoint = Vector3.zero;
				if (vectors.Count > (i + 1))
					nextPoint = vectors[i + 1];
				else
					nextPoint = vectors[0];
				optimalRoute.Add(nextPoint);
				usedPoints.Add(nextPoint);
				if ((i + 1) == numVectors)
				{
					for(int j = 0; j < startingIndex; j++)
					{
						Vector3 remainingPoint = vectors[j];
						if (!optimalRoute.Contains(remainingPoint) && !usedPoints.Contains(remainingPoint))
						{
							optimalRoute.Add(remainingPoint);
							usedPoints.Add(remainingPoint);
						}
					}
					break;
				}
			}
		}

		routeVectors = optimalRoute;
	}

	public void SetManeuverVector(Vector3 destination)
	{
		Vector3 toDestination = destination - transform.position;
		if (toDestination.magnitude >= 1f)
			spacecraft.Maneuver(toDestination);
	}

	void FlyTo(Vector3 destination)
	{
		SetManeuverVector(destination);

		/// thrust
		Vector3 toDestination = destination - rb.velocity - transform.position;
		if ((destination != holdPosition) && !bEngineActive)
		{
			Vector3 myVelocity = spacecraft.GetComponent<Rigidbody>().velocity;
			float distance = toDestination.magnitude;
			float speed = myVelocity.magnitude;
			float dotToDestination = Vector3.Dot(transform.forward, toDestination.normalized);
			if (dotToDestination > 0.99f)
			{
				if (distance > speed)
				{
					if (speed < spacecraft.mainEnginePower)
						speed = spacecraft.mainEnginePower;
					float thrustTime = (distance / speed) / 9f;
					engineCoroutine = MainEngineBurn(thrustTime);
					StartCoroutine(engineCoroutine);
				}
			}
		}
	}

	public void FireEngineBurn(float burnDuration)
	{
		if (bEngineActive)
			StopAllCoroutines();
		engineCoroutine = MainEngineBurn(burnDuration);
		StartCoroutine(engineCoroutine);
	}

	IEnumerator MainEngineBurn(float durationTime)
	{
		Debug.Log("Starting burn of " + durationTime + " seconds at " + Time.time.ToString("F1"));
		bEngineActive = true;
		spacecraft.MainEngines(1f);
		float timeElapsed = 0f;
		while (timeElapsed < durationTime)
		{
			timeElapsed += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		spacecraft.MainEngines(0f);
		bEngineActive = false;
	}

	public void ClearRoute()
	{
		if (routeVectors == null)
			routeVectors = new List<Vector3>();
		if (routeVectors != null)
			routeVectors.Clear();
		if (routeVisualizer != null)
		{
			routeVisualizer.ClearLine(-1);
			routeVisualizer.SetRouteColor(Color.white);
		}
		previousRouteVector = transform.position;

		if (spacecraft == null)
			spacecraft = GetComponent<Spacecraft>();
	}
}
