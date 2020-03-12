using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public LineRenderer trajectoryLinePrefab;
	public bool bAutoOrbit = false;

	private CircleRenderer circleRenderer;
	private Autopilot autopilot;
	private Planet planet;
	private GravityTelemetryHUD gravityTelemetry;
	private Vector3 inputVector = Vector3.zero;
	private float orbitRange = 30f;
	private float planetRadius = 0f;
	private float burnDuration = 0f;

	private List<LineRenderer> lineList;
	private List<Vector3> trajectoryList;
	public List<Vector3> GetTrajectory() { return trajectoryList; }
	public Vector3 GetOrbitTarget() { return inputVector; }

	void Awake()
    {
		lineList = new List<LineRenderer>();
		trajectoryList = new List<Vector3>();
		circleRenderer = GetComponentInChildren<CircleRenderer>();
		planet = FindObjectOfType<Planet>();
		gravityTelemetry = FindObjectOfType<GravityTelemetryHUD>();
	}

	public void SetAutopilot(Autopilot ap)
	{
		autopilot = ap;
	}

	public void SetBurnDuration(float value)
	{
		burnDuration = value;
		Debug.Log("burn duration " + value);
	}

	public void SetDirection(Vector3 direction)
	{
		if (bAutoOrbit)
		{
			inputVector = new Vector3(direction.x, direction.z, direction.y);
			transform.rotation = Quaternion.LookRotation(autopilot.gameObject.transform.position, direction);
		}
		else
		{
			if (!planet)
				planet = FindObjectOfType<Planet>();
			Vector3 toPlanet = planet.transform.position - autopilot.gameObject.transform.position;
			Vector3 inputToPlanet = Vector3.ProjectOnPlane(direction, toPlanet.normalized);

			planetRadius = planet.planetMesh.GetComponent<MeshRenderer>().bounds.max.magnitude;
			inputVector = inputToPlanet.normalized * ((planetRadius + orbitRange) / 2);

			Debug.DrawLine(autopilot.gameObject.transform.position, inputVector, Color.green);

			SimulateTrajectory(inputVector);
			RenderTrajectory();
		}
	}

	void SimulateTrajectory(Vector3 heading)
	{
		RecallTrajectoryLines();

		Spacecraft spacecraft = autopilot.gameObject.GetComponent<Spacecraft>();
		Rigidbody spacecraftRb = spacecraft.gameObject.GetComponent<Rigidbody>();
		float simulationTime = 0f;

		Vector3 currentPosition = spacecraft.transform.position;
		Vector3 velocityDirection = (heading - spacecraft.gameObject.transform.position).normalized;
		List<Gravity> gravityList = gravityTelemetry.GetGravitiesAffecting(spacecraftRb);
		trajectoryList.Add(currentPosition);

		while (simulationTime < burnDuration)
		{
			simulationTime += 0.1f;
			Vector3 velocity = velocityDirection * spacecraft.mainEnginePower;
			if (gravityList.Count > 0)
			{
				float spacecraftMass = spacecraftRb.mass;
				foreach (Gravity gr in gravityList)
					velocity += gr.GetGravity(spacecraftRb, currentPosition, spacecraftMass) * 0.1f;
			}
			currentPosition += velocity;
			trajectoryList.Add(currentPosition);
			velocityDirection = velocity;
		}
	}

	void RenderTrajectory()
	{
		int trajectoryCount = trajectoryList.Count;
		if (trajectoryCount > 0)
		{
			for(int i = 0; i < trajectoryCount; i += 2)
			{
				LineRenderer line = null;
				if ((i < lineList.Count) && (lineList[i] != null))
					line = lineList[i];
				else
					line = SpawnTrajectoryLine();

				Vector3 lineStart = trajectoryList[i];
				if (trajectoryList.Count > (i + 1))
				{
					Vector3 lineEnd = trajectoryList[i + 1];
					line.SetPosition(0, lineStart);
					line.SetPosition(1, lineEnd);
					line.enabled = true;
				}
			}
		}
	}

	LineRenderer SpawnTrajectoryLine()
	{
		LineRenderer line = Instantiate(trajectoryLinePrefab, transform);
		lineList.Add(line);
		return line;
	}

	void RecallTrajectoryLines()
	{
		if (lineList.Count > 0)
		{
			foreach(LineRenderer lr in lineList)
			{
				lr.enabled = false;
				lr.transform.localPosition = Vector3.zero;
			}
		}

		int numVectors = trajectoryList.Count;
		if (numVectors > 0)
		{
			for (int i = 0; i < numVectors; i++)
				trajectoryList[i] = Vector3.zero;
		}
		trajectoryList.Clear();
	}

	public void SetOrbitRange(float value)
	{
		if (bAutoOrbit)
		{
			circleRenderer.SetRadius(value);
		}
	}

	public void ModifyOrbitRange(float increment)
	{
		bool toeTest = (GetPoints()[0].magnitude > 1.6f);
		if (toeTest)
		{
			if (bAutoOrbit)
			{
				planetRadius = planet.planetMesh.GetComponent<MeshRenderer>().bounds.max.magnitude;
				float nextRadius = planetRadius + orbitRange + increment;
				circleRenderer.SetRadius(nextRadius);
				autopilot.SetRoute(GetPoints());
				autopilot.EnableMoveCommand(true);
			}
			else
			{
				//
			}
		}
	}

	public List<Vector3> GetPoints()
	{
		List<Vector3> pointList = new List<Vector3>();
		LineRenderer lineRenderer = circleRenderer.GetLineRenderer();
		int numPositions = lineRenderer.positionCount;
		Vector3[] vArray = new Vector3[numPositions];
		lineRenderer.GetPositions(vArray);
		if (vArray.Length > 0)
		{
			foreach(Vector3 vector in vArray)
				pointList.Add(transform.TransformPoint(vector));
		}
		return pointList;
	}
}
