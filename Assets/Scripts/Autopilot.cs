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

		routeVectors = new List<Vector3>();
		holdPosition = transform.position;
		targetCombatPosition = transform.position;
		autoPilotStartPosition = transform.position;
		previousRouteVector = transform.position;
		SetMoveCommand(transform.position);
	}

	public void SpacecraftNavigationCommands()
	{
		if (bExecutingMoveCommand && (routeVectors.Count > 0))
		{
			Vector3 currentRouteVector = routeVectors.ElementAt(0);
			FlyTo(currentRouteVector);

			if ((routeVectors.Count > 1)
				&& (Vector3.Distance(transform.position, currentRouteVector) < 1f))
			{
				routeVectors.Remove(currentRouteVector);
				routeVisualizer.ClearLine(0);
			}
		}
		else if (holdPosition != Vector3.zero)
		{
			FlyTo(holdPosition);
		}
	}

	public void SetTarget(Transform value)
	{
		targetTransform = value;
	}

	public void SetMoveCommand(Vector3 value)
	{
		commandMoveVector = value;
		if (commandMoveVector.magnitude > 1f)
		{
			GenerateRoute();
		}
		else
		{
			ClearRoute();
		}
	}

	public void EnableMoveCommand(bool value)
	{
		bExecutingMoveCommand = value;
		if (value)
			routeVisualizer.SetRouteColor(Color.green);
	}

	public void SetFollowTransform(Transform value)
	{
		if (value != null)
		{
			followTransform = value;
		}
	}

	void GenerateRoute()
	{
		ClearRoute();
		for (int i = 0; i < routeVectorPoints; i++)
		{
			Vector3 routeStep = GenerateRouteVector();
			routeVectors.Add(routeStep);

			if ((spacecraft.GetMarks() > 0) || (spacecraft.GetAgent().teamID == 0))
				routeVisualizer.SetLine(i, previousRouteVector, routeStep);

			previousRouteVector = routeStep;
		}
	}

	Vector3 GenerateRouteVector()
	{
		Vector3 velocity = Vector3.zero;

		// move command
		if (commandMoveVector != Vector3.zero)
			velocity += (commandMoveVector - transform.position) / routeVectorPoints;

		// gravity negotiation
		Vector3 gravityEscapeRoute = GetGravityEscapeFrom(previousRouteVector + velocity);
		//velocity += gravityEscapeRoute;

		// follow command
		//if (followTransform != null)
		//{
		//	Vector3 followingRoute = GetFollowPosition() - previousRouteVector;
		//	velocity += followingRoute;
		//}

		return previousRouteVector + velocity;
	}

	void FlyTo(Vector3 destination)
	{
		// steering
		Vector3 toDestination = destination - transform.position;
		spacecraft.Maneuver(destination);

		// counteracting unwanted velocity
		Vector3 myVelocity = rb.velocity;
		float velocityDotToTarget = Vector3.Dot(myVelocity.normalized, toDestination.normalized);
		if ((velocityDotToTarget <= 0.95f) || (toDestination.magnitude <= 1f))
		{
			Vector3 unwantedVelocity = Vector3.ProjectOnPlane(rb.velocity, toDestination.normalized);
			maneuverVector = unwantedVelocity * -1 * spacecraft.maneuverPower * (1f - velocityDotToTarget);
			spacecraft.ManeuverEngines(maneuverVector);
			Debug.DrawRay(transform.position, maneuverVector, Color.black);
		}

		// thrust
		float dotToTarget = Vector3.Dot(transform.forward, toDestination.normalized);
		if (dotToTarget > 0f)
		{
			float thrustPower = Mathf.Pow(Mathf.Abs(dotToTarget), 2f);
			thrustPower *= toDestination.magnitude / 10f;
			thrustPower = Mathf.Clamp(thrustPower, 0f, 1f);
			spacecraft.MainEngines(thrustPower);
		}
	}

	Vector3 GetGravityEscapeFrom(Vector3 position)
	{
		Vector3 gravityEscape = Vector3.zero;
		foreach (Gravity g in objectManager.GetGravityList())
		{
			RaycastHit hit;
			if (Physics.Raycast(position, (g.transform.position - position), out hit))
			{
				if ((hit.transform == g.transform) && (hit.distance < g.radius))
				{
					float dangerScale = (3.14f / hit.distance);
					gravityEscape =  hit.point + (hit.normal * spacecraft.mainEnginePower * dangerScale);
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
