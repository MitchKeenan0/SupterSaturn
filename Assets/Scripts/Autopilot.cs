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

	private Agent agent;
	private Rigidbody rb;
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
	private float timeAtLastMoveCommand = 0;
	private List<Vector3> routeVectors;

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
			if ((distance < 5f) && ((Time.time - timeAtLastMoveCommand) > 1f))
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
					routeVisualizer.SetLine(i, previous, routeStep);
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

		// get closest first
		float closestDistance = 99999f;
		Vector3 closestPoint = Vector3.zero;
		for (int i = 0; i < numVectors; i++)
		{
			Vector3 thisVector = vectors[i];
			float thisDistance = Vector3.Distance(spacecraft.transform.position, thisVector);
			if ((thisDistance < closestDistance) && (!usedPoints.Contains(thisVector)))
			{
				closestDistance = thisDistance;
				closestPoint = thisVector;
			}
		}
		optimalRoute.Add(closestPoint);
		usedPoints.Add(closestPoint);

		// follow through with dot product
		float tries = 0;
		while ((optimalRoute.Count < vectors.Count) && (tries < 300))
		{
			tries++;
			for (int i = 0; i < numVectors; i++)
			{
				float closeF = 99999f;
				Vector3 closeV = Vector3.zero;
				Vector3 prevPos = Vector3.zero;
				for (int j = 0; j < numVectors; j++)
				{
					Vector3 vector = vectors[i];
					float thisDistance = Vector3.Distance(prevPos, vector);
					if ((thisDistance < closeF) && (!usedPoints.Contains(vector)))
					{
						closeF = thisDistance;
						closeV = vector;
					}
				}
				
				Vector3 thisVector = vectors[i];
				Vector3 movingDelta = closeV - prevPos;
				float thisDot = Vector3.Dot(movingDelta.normalized, transform.right.normalized);
				if (thisDot > 0.0f)
				{
					optimalRoute.Add(closeV);
					usedPoints.Add(closeV);
					prevPos = closeV;
				}
			}
		}
		routeVectors = optimalRoute;
	}

	void FlyTo(Vector3 destination)
	{
		Debug.DrawLine(transform.position, destination, Color.green);

		Vector3 toDestination = destination - rb.velocity - transform.position;

		// steering
		if (toDestination.magnitude >= 1f)
		{
			Vector3 maneuverVector = destination - transform.position;
			spacecraft.Maneuver(maneuverVector);
		}

		// side jets setup
		Vector3 frameVelocity = rb.velocity.normalized;
		Vector3 frameDestination = toDestination.normalized;
		Vector3 driveVector = Vector3.zero;
		if (frameVelocity.x != frameDestination.x)
			driveVector.x = frameDestination.x - frameVelocity.x;
		if (frameVelocity.y != frameDestination.y)
			driveVector.y = frameDestination.y - frameVelocity.y;
		if (frameVelocity.z != frameDestination.z)
			driveVector.z = frameDestination.z - frameVelocity.z;
		driveVector.z = Mathf.Clamp(driveVector.z, -1f, 1f);
		driveVector = Vector3.ProjectOnPlane(driveVector, toDestination);
		if (driveVector != Vector3.zero)
		{
			driveVector = Vector3.ClampMagnitude(driveVector, spacecraft.maneuverPower);
			spacecraft.ManeuverEngines(driveVector);
		}

		// thrust
		if (destination != holdPosition)
		{
			float distanceToDestination = toDestination.magnitude;
			float dotToDestination = Vector3.Dot(transform.forward, toDestination.normalized);
			if (Mathf.Abs(dotToDestination) > 0.99f)
			{
				float throttle = Mathf.Clamp(((distanceToDestination * 0.1f) * spacecraft.mainEnginePower * dotToDestination), -1, 1);
				float destinationVelocityDot = Vector3.Dot(toDestination.normalized, rb.velocity.normalized);
				spacecraft.MainEngines(throttle);
			}
		}
	}

	void AllStop()
	{
		spacecraft.MainEngines(0f);
		spacecraft.ManeuverEngines(Vector3.zero);
		spacecraft.Maneuver(Vector3.zero);
	}

	Vector3 GetGravityEscapeFrom(Vector3 position)
	{
		Vector3 gravityEscape = Vector3.zero;
		foreach (Gravity g in objectManager.GetGravityList())
		{
			RaycastHit hit;
			Vector3 gravityVector = (g.transform.position - position).normalized;
			if (Physics.Raycast(position, gravityVector, out hit))
			{
				if ((hit.collider == g.GetComponent<Collider>()) && (hit.distance < g.radius))
				{
					gravityEscape = (-gravityVector * spacecraft.mainEnginePower * g.radius);
					//Debug.DrawRay(hit.point, gravityEscape, Color.green);
				}
			}
		}

		return gravityEscape;
	}

	Vector3 GetFollowPosition()
	{
		Vector3 follow = Vector3.zero;
		if (followTransform != null)
		{
			follow = followTransform.position;
		}
		return follow;
	}

	void ClearRoute()
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
