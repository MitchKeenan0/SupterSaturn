using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public LineRenderer trajectoryLinePrefab;
	public float updateInterval = 0.1f;

	private Autopilot autopilot;
	private Rigidbody rb;
	private Planet planet;
	private GravityTelemetryHUD gravityTelemetry;
	private Vector3 inputVector = Vector3.zero;
	
	private NavigationHud navigationHud;
	private List<LineRenderer> lineList;
	private List<Vector3> trajectoryList;
	public List<Vector3> GetTrajectory() { return trajectoryList; }
	private float planetRadius = 0f;
	private float burnDuration = 0f;
	private bool bUpdating = false;

	public Autopilot GetAutopilot() { return autopilot; }

	private IEnumerator updateCoroutine;

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
		rb = autopilot.gameObject.GetComponent<Rigidbody>();
	}

	public void SetBurnDuration(float value)
	{
		burnDuration = value;
	}

	public void SetDirection(Vector3 inputVector)
	{
		if (bUpdating)
			bUpdating = false;
		if (autopilot != null)
		{
			SimulateTrajectory(inputVector, burnDuration);
			RenderTrajectory();
		}
	}

	public void SetUpdating(bool value)
	{
		if (value && !bUpdating)
		{
			updateCoroutine = UpdateOrbit(updateInterval);
			StartCoroutine(updateCoroutine);
		}
		else if (bUpdating)
		{
			StopCoroutine(updateCoroutine);
		}
		bUpdating = value;
	}

	IEnumerator UpdateOrbit(float interval)
	{
		while (true)
		{
			if ((rb != null) && (rb.velocity.magnitude > 0f))
			{
				SimulateTrajectory(rb.velocity, 0f);
				RenderTrajectory();
			}
			yield return new WaitForSeconds(interval);
		}
	}

	void SimulateTrajectory(Vector3 heading, float duration)
	{
		ClearTrajectory();

		Spacecraft spacecraft = autopilot.gameObject.GetComponent<Spacecraft>();
		Vector3 currentPosition = spacecraft.transform.position;
		List<Gravity> gravityList = gravityTelemetry.GetGravitiesAffecting(rb);
		trajectoryList.Add(currentPosition);
		if (heading.magnitude < 1f)
			return;
		bool bSimulateEnginePower = true;
		if (duration == 0f)
		{
			duration = rb.velocity.magnitude;
			bSimulateEnginePower = false;
		}

		float simulationTime = 0f;
		float deltaTime = 0.1f;
		Vector3 velocity = currentPosition + (rb.velocity * deltaTime);
		Vector3 trajecto = spacecraft.transform.forward * deltaTime;
		float enginePower = (spacecraft.mainEnginePower / rb.mass) * deltaTime;
		while (simulationTime < duration)
		{
			Vector3 frameVelocity = trajecto * (velocity.magnitude * deltaTime) * deltaTime;
			if (bSimulateEnginePower)
				frameVelocity = trajecto * enginePower * deltaTime;
			velocity += frameVelocity;

			if (gravityList.Count > 0)
			{
				foreach(Gravity gr in gravityList)
					velocity += (gr.GetGravity(rb, currentPosition, rb.mass) * deltaTime) * deltaTime * 0.16f;
			}

			trajectoryList.Add(velocity);
			trajecto = (velocity - currentPosition).normalized;
			currentPosition = velocity;
			simulationTime += deltaTime;
		}
	}

	void RenderTrajectory()
	{
		int trajectoryCount = trajectoryList.Count;
		if (trajectoryCount > 0)
		{
			for(int i = 0; i < trajectoryCount; i++)
			{
				LineRenderer line = null;
				if ((i < lineList.Count) && (lineList[i] != null))
					line = lineList[i];
				else
					line = SpawnTrajectoryLine();

				Vector3 lineStart = trajectoryList[i];
				if (trajectoryList.Count > (i + 1))
				{
					Vector3 lineEnd = lineStart + ((trajectoryList[i + 1] - trajectoryList[i]) * 0.6f); ///trajectoryList[i + 1];
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
