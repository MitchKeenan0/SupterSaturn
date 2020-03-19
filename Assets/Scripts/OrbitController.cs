using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public LineRenderer trajectoryLinePrefab;
	public float orbitRange = 25f;
	private Autopilot autopilot;
	private Planet planet;
	private GravityTelemetryHUD gravityTelemetry;
	private Vector3 inputVector = Vector3.zero;
	
	private float planetRadius = 0f;
	private float burnDuration = 0f;
	private NavigationHud navigationHud;
	private List<LineRenderer> lineList;
	private List<Vector3> trajectoryList;
	public List<Vector3> GetTrajectory() { return trajectoryList; }

	public Autopilot GetAutopilot() { return autopilot; }

	void Awake()
    {
		lineList = new List<LineRenderer>();
		trajectoryList = new List<Vector3>();
		navigationHud = FindObjectOfType<NavigationHud>();
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
	}

	public void SetDirection(Vector3 inputVector)
	{
		if (autopilot != null)
		{
			SimulateTrajectory(inputVector);
			RenderTrajectory();
		}
	}

	void SimulateTrajectory(Vector3 heading)
	{
		ClearTrajectory();

		Spacecraft spacecraft = autopilot.gameObject.GetComponent<Spacecraft>();
		Rigidbody spacecraftRb = spacecraft.gameObject.GetComponent<Rigidbody>();
		float spacecraftMass = spacecraftRb.mass;
		float spacecraftDrag = spacecraftRb.drag;
		float simulationTime = 0f;
		Vector3 currentPosition = spacecraft.transform.position;
		Vector3 toHeading = (heading - currentPosition).normalized * 0.01f;
		Vector3 velocity = spacecraftRb.velocity;
		Vector3 forward = spacecraft.transform.forward;
		List<Gravity> gravityList = gravityTelemetry.GetGravitiesAffecting(spacecraftRb);
		trajectoryList.Add(currentPosition);

		float deltaTime = 0.01f;
		int iterations = 0;
		while ((simulationTime < burnDuration) && (iterations < 100))
		{
			simulationTime += deltaTime;
			iterations++;

			velocity = currentPosition + (forward * spacecraft.mainEnginePower * deltaTime);
			if (gravityList.Count > 0)
			{
				foreach (Gravity gr in gravityList)
					velocity += gr.GetGravity(spacecraftRb, currentPosition, spacecraftMass) * deltaTime * 0.6f;
			}
			
			trajectoryList.Add(velocity);
			forward = (velocity - currentPosition).normalized;
			currentPosition = velocity;
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

	void ClearTrajectory()
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
		//
	}

	public void ModifyOrbitRange(float increment)
	{
		//
	}
}
