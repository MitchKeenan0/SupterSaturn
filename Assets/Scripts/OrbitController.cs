using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public LineRenderer trajectoryLinePrefab;
	public float orbitRange = 25f;

	private CircleRenderer circleRenderer;
	private Autopilot autopilot;
	private Planet planet;
	private GravityTelemetryHUD gravityTelemetry;
	private Vector3 inputVector = Vector3.zero;
	
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
		Debug.Log("Set Autopilot");
	}

	public void SetBurnDuration(float value)
	{
		burnDuration = value;
		Debug.Log("burn duration " + burnDuration);
	}

	public void SetDirection(Vector3 onscreenPosition)
	{
		if (autopilot != null)
		{
			Camera cameraMain = Camera.main;
			Vector3 planetScreenPos = cameraMain.WorldToScreenPoint(Vector3.zero);
			Vector3 onscreenDirection = onscreenPosition - planetScreenPos;

			Vector3 inputToPlanet = (Quaternion.Euler(cameraMain.transform.eulerAngles) * onscreenDirection) - autopilot.transform.position;
			inputVector = inputToPlanet;

			Debug.DrawLine(Vector3.zero, inputToPlanet, Color.yellow);

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
		heading += spacecraftRb.velocity;

		Vector3 currentPosition = spacecraft.transform.position;
		Vector3 velocityDirection = (heading - spacecraft.gameObject.transform.position).normalized;
		Vector3 velocity = spacecraftRb.velocity;
		List<Gravity> gravityList = gravityTelemetry.GetGravitiesAffecting(spacecraftRb);
		trajectoryList.Add(currentPosition);

		while (simulationTime < burnDuration)
		{
			simulationTime += 0.1f;
			velocity += velocityDirection * spacecraft.mainEnginePower * 0.1f;
			if (gravityList.Count > 0)
			{
				foreach (Gravity gr in gravityList)
					velocity += gr.GetGravity(spacecraftRb, currentPosition, spacecraftMass, spacecraftDrag) * (1f - spacecraftDrag);
			}
			currentPosition += velocity;
			trajectoryList.Add(currentPosition);
			velocityDirection = velocity.normalized;
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
		bool toeTest = (GetCirclePoints()[0].magnitude > 1.6f);
		if (toeTest)
		{
			//
		}
	}

	public List<Vector3> GetCirclePoints()
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
