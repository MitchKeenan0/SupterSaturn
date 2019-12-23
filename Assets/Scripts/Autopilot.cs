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
	private Vector3 gravityCommandMoveVector = Vector3.zero;
	private Vector3 targetCombatPosition = Vector3.zero;
	private Vector3 prevStablePosition = Vector3.zero;
	private Vector3 autoPilotStartPosition = Vector3.zero;
	private Vector3 maneuverVector = Vector3.zero;
	private Vector3 previousRouteVector = Vector3.zero;
	private bool bInGravity = false;
	private List<Vector3> routeVectors;

	void Start()
    {
		rb = GetComponent<Rigidbody>();
		agent = GetComponent<Agent>();
		spacecraft = GetComponent<Spacecraft>();
		routeVisualizer = GetComponentInChildren<RouteVisualizer>();
		objectManager = FindObjectOfType<ObjectManager>();

		routeVectors = new List<Vector3>();
		targetCombatPosition = transform.position;
		autoPilotStartPosition = transform.position;
		previousRouteVector = transform.position;
		SetMoveCommand(transform.position);
	}

	public void SpacecraftNavigationCommands()
	{
		Debug.DrawLine(transform.position, commandMoveVector, Color.blue);

		if ((followTransform != null) && 
			(routeVectors.Count <= 1) &&
			(Vector3.Distance(followTransform.position, transform.position) >= 3f))
		{
			GenerateRoute();
		}
		if (routeVectors.Count > 0)
		{
			Vector3 currentRouteVector = routeVectors.ElementAt(0);
			FlyTo(currentRouteVector);

			if ((routeVectors.Count > 1) 
				&& ((Vector3.Distance(transform.position, currentRouteVector) < 1f) 
					|| (Vector3.Dot((currentRouteVector - transform.position).normalized, transform.forward) < 0.5f)))
			{
				routeVectors.Remove(currentRouteVector);
				routeVisualizer.ClearLine(0);
			}
		}
		else
		{
			FlyTo(transform.position);
		}
	}

	public void SetTarget(Transform value)
	{
		targetTransform = value;
	}

	public void SetMoveCommand(Vector3 value)
	{
		commandMoveVector = value - transform.position;
		if (commandMoveVector.magnitude > 1f)
		{
			GenerateRoute();
		}
		else
		{
			ClearRoute();
		}
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
			Vector3 routeStep = GenerateRouteVector(i);
			routeVectors.Add(routeStep);
			if ((spacecraft.GetMarks() > 0) || (spacecraft.GetAgent().teamID == 0))
				routeVisualizer.SetLine(i, previousRouteVector, routeStep);
			previousRouteVector = routeStep;
		}
	}

	Vector3 GenerateRouteVector(int linePositionIndex)
	{
		Vector3 newRouteVector = Vector3.zero;
		Vector3 velocity = Vector3.zero;

		// gravity negotiation
		Vector3 gravityEscapeRoute = GetGravityEscapeFrom(previousRouteVector);
		velocity += gravityEscapeRoute - previousRouteVector;

		// move command
		if (bInGravity)
			velocity += (gravityCommandMoveVector / routeVectorPoints);
		else
			velocity += (commandMoveVector / routeVectorPoints);

		// follow command
		if (followTransform != null)
		{
			Vector3 followingRoute = GetFollowPosition() - previousRouteVector;
			velocity += followingRoute;
		}

		newRouteVector.x = previousRouteVector.x + velocity.x * Time.deltaTime * 100f;
		newRouteVector.y = previousRouteVector.y + velocity.y * Time.deltaTime * 100f;
		newRouteVector.z = previousRouteVector.z + velocity.z * Time.deltaTime * 100f;

		return newRouteVector;
	}

	void FlyTo(Vector3 destination)
	{
		// steering
		Vector3 toDestination = destination - transform.position;
		spacecraft.Maneuver(destination);

		// counteracting unwanted velocity
		Vector3 myVelocity = rb.velocity;
		float velocityDotToTarget = Vector3.Dot(myVelocity.normalized, toDestination.normalized);
		if (velocityDotToTarget <= 0.95f)
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
			spacecraft.MainEngines(thrustPower);
		}
	}

	Vector3 GetGravityEscapeFrom(Vector3 position)
	{
		bInGravity = false;
		Vector3 gravityEscape = Vector3.zero;

		foreach (Gravity g in objectManager.GetGravityList())
		{
			RaycastHit hit;
			if (Physics.Raycast(position, (g.transform.position - position), out hit))
			{
				if ((hit.transform == g.transform)
					&& (hit.distance < g.radius))
				{
					float dangerScale = (3.14f / hit.distance);
					gravityEscape = (position - g.transform.position) * spacecraft.mainEnginePower * dangerScale;
					bInGravity = true;

					RaycastHit commandMoveHit;
					if (Physics.Raycast(position, commandMoveVector - transform.position, out commandMoveHit))
					{
						gravityCommandMoveVector = Vector3.ProjectOnPlane(commandMoveVector, hit.normal);
					}
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
		if (!bInGravity)
		{
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
		previousRouteVector = transform.position;
	}
}
