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
	private float burnDuration = 0f;
	private bool bUpdating = false;

	public Autopilot GetAutopilot() { return autopilot; }
	public List<Vector3> GetTrajectory() { return trajectoryList; }

	void Awake()
    {
		lineList = new List<LineRenderer>();
		trajectoryList = new List<Vector3>();
		navigationHud = FindObjectOfType<NavigationHud>();
		planet = FindObjectOfType<Planet>();
		gravityTelemetry = FindObjectOfType<GravityTelemetryHUD>();
		for (int i = 0; i < 300; i++)
			SpawnTrajectoryLine();
	}

	public void SetAutopilot(Autopilot ap)
	{
		autopilot = ap;
		if (autopilot != null)
			rb = autopilot.gameObject.GetComponent<Rigidbody>();
	}

	public void SetBurnDuration(float value)
	{
		burnDuration = value;
	}

	public void SetDirection(Vector3 inputVector)
	{
		bUpdating = false;
		if (autopilot != null)
		{
			///autopilot.FaceVelocity(false);
			SimulateTrajectory(inputVector, burnDuration);
			RenderTrajectory();
		}
	}

	public void SetUpdating(bool value)
	{
		bUpdating = value;
	}

	public void SetUpdatingForDuration(float value)
	{
		durationUpdateCoroutine = DurationUpdate(value);
		StartCoroutine(durationUpdateCoroutine);
	}

	private IEnumerator durationUpdateCoroutine;
	private IEnumerator DurationUpdate(float duration)
	{
		bUpdating = true;
		yield return new WaitForSeconds(duration);
		bUpdating = false;
	}

	void Update()
	{
		if (bUpdating)
		{
			if ((rb != null) && (rb.velocity.magnitude > 0f))
			{
				SimulateTrajectory(rb.velocity, -1f);
				RenderTrajectory();
			}
		}
	}

	void SimulateTrajectory(Vector3 heading, float duration)
	{
		ClearTrajectory();

		Spacecraft spacecraft = autopilot.gameObject.GetComponent<Spacecraft>();
		bool bSimulateEnginePower = true;
		if (duration <= 0f)
		{
			duration = Mathf.Clamp(rb.velocity.magnitude * 10, 1f, 30f);
			bSimulateEnginePower = false;
		}

		float simulationTime = 0f;
		float deltaTime = 0.1f;
		Vector3 currentPosition = spacecraft.transform.position;
		Vector3 deltaVelocity = rb.velocity;
		Vector3 trajecto = Vector3.zero;
		Vector3 velocity = Vector3.zero;
		List<Gravity> gravityList = gravityTelemetry.GetGravitiesAffecting(rb);
		trajectoryList.Add(currentPosition);

		while (simulationTime < duration)
		{
			trajecto = deltaVelocity;
			if (bSimulateEnginePower)
				trajecto += spacecraft.transform.forward * (1f / deltaTime);

			int numGravs = gravityList.Count;
			for (int i = 0; i < numGravs; i++)
			{
				Gravity gr = gravityList[i];
				trajecto += gr.GetGravity(rb, currentPosition, rb.mass);
			}

			velocity = currentPosition + trajecto;

			trajectoryList.Add(velocity);
			deltaVelocity = (velocity - currentPosition);
			currentPosition = velocity;
			simulationTime += deltaTime;
		}
	}

	void RenderTrajectory()
	{
		int trajectoryCount = trajectoryList.Count;
		for (int i = 0; i < trajectoryCount; i++)
		{
			LineRenderer line = null;
			if ((i < lineList.Count) && (lineList[i] != null))
			{
				line = lineList[i];
			}

			if (line != null)
			{
				Vector3 lineStart = trajectoryList[i];
				if (trajectoryList.Count > (i + 1))
				{
					Vector3 lineEnd = lineStart + ((trajectoryList[i + 1] - trajectoryList[i]) * 0.6f);
					line.SetPosition(0, lineStart);
					line.SetPosition(1, lineEnd);
					line.enabled = true;

					float normal = Mathf.InverseLerp(0f, trajectoryCount, i);
					float lineAlpha = Mathf.Lerp(0.9f, 0.6f, Mathf.Sqrt(normal));
					Color lineColor = new Color(0f, lineAlpha, 0f);
					Color start = lineColor * 0.8f;
					start.a = 1f;
					line.startColor = start;
					line.endColor = lineColor;

					if (Physics.Linecast(lineStart, lineEnd))
					{
						return;
					}
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

	public void ClearTrajectory()
	{
		if (lineList.Count > 0)
		{
			foreach(LineRenderer lr in lineList)
			{
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
