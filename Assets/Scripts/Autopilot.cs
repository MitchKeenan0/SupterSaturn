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

			if ((routeVectors.Count >= 1)
				&& (Vector3.Distance(transform.position, currentRouteVector) < 1f))
			{
				holdPosition = currentRouteVector;
				routeVectors.Remove(currentRouteVector);
				routeVisualizer.ClearLine(0);
			}
		}
		else if (holdPosition != Vector3.zero)
		{
			if (Vector3.Distance(transform.position, holdPosition) > 0.1f)
			{
				FlyTo(holdPosition);
			}
		}
	}

	public void SetTarget(Transform value)
	{
		targetTransform = value;
	}

	public void SetMoveCommand(Vector3 value)
	{
		commandMoveVector = value;
		if (Vector3.Distance(commandMoveVector, transform.position) >= 1f)
		{
			GenerateRoute();
		}
	}

	public void EnableMoveCommand(bool value)
	{
		bExecutingMoveCommand = value;
		if (bExecutingMoveCommand && (routeVisualizer != null))
		{
			routeVisualizer.SetRouteColor(Color.green);
		}
		else if (spacecraft != null)
		{
			routeVisualizer.SetRouteColor(Color.clear);
			holdPosition = spacecraft.transform.position;
		}
	}

	public void SetFollowTransform(Transform value)
	{
		followTransform = value;
	}

	void GenerateRoute()
	{
		ClearRoute();
		for (int i = 0; i < routeVectorPoints; i++)
		{
			Vector3 routeStep = GenerateRouteVector();
			routeVectors.Add(routeStep);

			if ((spacecraft.GetMarks() > 0) || (spacecraft.GetAgent().teamID == 0))
				routeVisualizer.SetLine(i, transform.position, routeStep);

			previousRouteVector = routeStep;
		}
	}

	Vector3 GenerateRouteVector()
	{
		Vector3 velocity = previousRouteVector;

		// move command
		if (commandMoveVector != Vector3.zero)
			velocity += (commandMoveVector - transform.position) / routeVectorPoints;

		// squad position
		if (selectionSquad != null)
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
		// calibrate for current velocity
		Vector3 dynamicDestination = destination;
		if (Mathf.Abs(rb.velocity.magnitude) > 0f)
			dynamicDestination += (-rb.velocity);
		Vector3 toDestination = dynamicDestination - transform.position;

		// steering
		if (toDestination.magnitude > 0.5f)
			spacecraft.Maneuver(dynamicDestination);

		// counteracting unwanted velocity
		//if (toDestination.magnitude > 0.01f)
		//{
		//	Vector3 unwantedVelocity = Vector3.ProjectOnPlane(rb.velocity, toDestination.normalized);
		//	Vector3 counterVector = (destination - unwantedVelocity).normalized;
		//	float velocityDotToTarget = Vector3.Dot(rb.velocity.normalized, toDestination.normalized);
		//	maneuverVector = counterVector * spacecraft.maneuverPower;
		//	spacecraft.ManeuverEngines(maneuverVector);
		//}

		// thrust
		Vector3 direction = (destination - rb.velocity) - transform.position;
		Debug.DrawRay(transform.position, direction, Color.green);
		if (direction.magnitude > 0.05f)
		{
			float dotToTarget = Vector3.Dot(transform.forward, toDestination.normalized);
			if (Mathf.Abs(dotToTarget) > 0.97f)
			{
				float distanceToDestination = direction.magnitude;
				float currentSpeed = rb.velocity.magnitude;
				float throttle = (distanceToDestination - currentSpeed) * spacecraft.mainEnginePower;
				throttle = Mathf.Clamp(throttle, -1f, 1f);
				spacecraft.MainEngines(throttle);
			}
		}
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
					Debug.DrawRay(hit.point, gravityEscape, Color.green);
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
		routeVectors.Clear();
		routeVisualizer.ClearLine(-1);
		routeVisualizer.SetRouteColor(Color.grey);
		previousRouteVector = transform.position;
	}
}
