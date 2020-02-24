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
	private Transform targetTransform;
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
			if ((distance < 0.5f) && ((Time.time - timeAtLastMoveCommand) > 1f))
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

	public void SetTarget(Transform value)
	{
		targetTransform = value;
	}

	public void SetMoveCommand(Vector3 value, bool squadOffset)
	{
		commandMoveVector = value;
		if (Vector3.Distance(commandMoveVector, transform.position) > 0.1f)
			GenerateRoute(squadOffset);
	}

	public void EnableMoveCommand(bool value)
	{
		bExecutingMoveCommand = value;
		if (!routeVisualizer)
			routeVisualizer = GetComponentInChildren<RouteVisualizer>();
		if (routeVisualizer != null)
		{
			LineMeasureHUD lineMeasure = routeVisualizer.gameObject.GetComponentInChildren<LineMeasureHUD>();
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

	void GenerateRoute(bool squadOffset)
	{
		ClearRoute();
		if (!routeVisualizer)
			routeVisualizer = GetComponentInChildren<RouteVisualizer>();
		for (int i = 0; i < routeVectorPoints; i++)
		{
			Vector3 routeStep = GenerateRouteVector(squadOffset);
			routeVectors.Add(routeStep);
			if (((spacecraft.GetMarks() > 0) || (spacecraft.GetAgent().teamID == 0))
				&& (routeVisualizer != null))
				routeVisualizer.SetLine(i, transform.position, routeStep);
			previousRouteVector = routeStep;
		}
	}

	Vector3 GenerateRouteVector(bool squadOffset)
	{
		Vector3 velocity = previousRouteVector;

		// move command
		if (commandMoveVector != Vector3.zero)
			velocity += (commandMoveVector - transform.position) / routeVectorPoints;

		// squad position
		if (squadOffset && (selectionSquad != null))
			velocity += selectionSquad.GetOffsetOf(spacecraft) / routeVectorPoints;

		// gravity negotiation
		//Vector3 gravityEscapeRoute = GetGravityEscapeFrom(velocity);
		//velocity += gravityEscapeRoute;

		// follow command
		//if (followTransform != null)
		//{
		//	Vector3 followingRoute = GetFollowPosition() - previousRouteVector;
		//	velocity += followingRoute;
		//}

		return velocity;
	}

	void FlyTo(Vector3 destination)
	{
		Debug.DrawLine(transform.position, destination, Color.green);

		Vector3 toDestination = destination - rb.velocity - transform.position;

		// steering
		if (toDestination.magnitude >= 1.62f)
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
			if ((distanceToDestination > 0.1f) && (destination != holdPosition))
			{
				float dotToDestination = Vector3.Dot(transform.forward, toDestination.normalized);
				if (Mathf.Abs(dotToDestination) > 0.99f)
				{
					float throttle = Mathf.Clamp(((distanceToDestination * 0.1f) * spacecraft.mainEnginePower * dotToDestination), -1, 1);
					float destinationVelocityDot = Vector3.Dot(toDestination.normalized, rb.velocity.normalized);
					spacecraft.MainEngines(throttle);
				}
				else if (dotToDestination < 0f)
				{
					spacecraft.MainEngines(dotToDestination);
				}
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

	Vector3 GetCombatPosition()
	{
		Vector3 combatPos = Vector3.zero;
		if (targetTransform != null)
		{
			// combat position
			Vector3 toTarget = targetTransform.position - transform.position;
			if (toTarget.magnitude > (agent.preferredEngagementDistance * 1.6f))
			{
				combatPos += toTarget.normalized * targetCombatPosition.magnitude;
			}
			else if (toTarget.magnitude < (agent.preferredEngagementDistance / 1.6f))
			{
				combatPos -= toTarget;
			}

			// evasion
			Vector3 crossFade = Vector3.Project(toTarget, transform.right);
			combatPos += crossFade;

			// chase / feint
			Vector3 targetVelocity = targetTransform.GetComponent<Rigidbody>().velocity;
			if (targetVelocity.magnitude > rb.velocity.magnitude)
			{
				Vector3 combatOffset = Random.insideUnitCircle * autopilotSkill;
				combatPos += (targetTransform.position + targetVelocity);
			}
		}

		return combatPos;
	}

	Vector3 GetAutopilotSkillPosition()
	{
		Vector3 randomDeviation = Random.insideUnitSphere * (0.1f / autopilotSkill);
		return randomDeviation;
	}

	void ClearRoute()
	{
		if (routeVectors != null)
			routeVectors.Clear();
		if (routeVisualizer != null)
		{
			routeVisualizer.ClearLine(-1);
			routeVisualizer.SetRouteColor(Color.white);
		}
		previousRouteVector = transform.position;
	}
}
