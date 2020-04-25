using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public LineRenderer trajectoryLinePrefab;
	public int maxLineCount = 250;
	public float updateInterval = 0.1f;

	private Autopilot autopilot;
	private Rigidbody rb;
	private Planet planet;
	private GravityTelemetryHUD gravityTelemetry;
	private Vector3 inputVector = Vector3.zero;
	
	private NavigationHud navigationHud;
	private Camera cameraMain;
	private List<LineRenderer> lineList;
	private List<Vector3> trajectoryList;
	private float burnDuration = 0f;
	private bool bUpdating = false;
	private bool bBackgroundUpdating = false;
	private int trajectoryClampedLength = 0;

	public Autopilot GetAutopilot() { return autopilot; }
	public List<Vector3> GetTrajectory() { return trajectoryList; }
	public void SetClampedLength(int value) { trajectoryClampedLength = (value != 0) ? value : maxLineCount; }

	void Awake()
    {
		lineList = new List<LineRenderer>();
		trajectoryList = new List<Vector3>();
		navigationHud = FindObjectOfType<NavigationHud>();
		planet = FindObjectOfType<Planet>();
		gravityTelemetry = FindObjectOfType<GravityTelemetryHUD>();
		cameraMain = Camera.main;
		trajectoryClampedLength = maxLineCount;
		for (int i = 0; i < maxLineCount; i++)
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
		//bUpdating = false;
		//if (bBackgroundUpdating)
			//StopCoroutine(backgroundCoroutine);
		//bBackgroundUpdating = false;
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
		bool previous = bUpdating;
		bUpdating = true;
		yield return new WaitForSeconds(duration);
		if (!previous)
			DelayStop(0.2f);
		else
			bUpdating = previous;
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
			duration = Mathf.Clamp(rb.velocity.magnitude, 0.6f, 30f);
			bSimulateEnginePower = false;
		}

		float simulationTime = 0f;
		float deltaTime = 0.1f;
		Vector3 currentPosition = spacecraft.transform.position;
		Vector3 deltaVelocity = rb.velocity;
		Vector3 trajecto = Vector3.zero;
		Vector3 velocity = Vector3.zero;
		List<Gravity> gravityList = new List<Gravity>();
		gravityList = gravityTelemetry.GetGravitiesAffecting(rb);
		trajectoryList.Add(currentPosition);

		while (simulationTime < duration)
		{
			trajecto = deltaVelocity;
			if (bSimulateEnginePower)
				trajecto += spacecraft.transform.forward * (spacecraft.mainEnginePower * deltaTime);

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

			if ((i < trajectoryClampedLength) && (line != null))
			{
				Vector3 lineStart = trajectoryList[i];
				if (trajectoryList.Count > (i + 1))
				{
					Vector3 lineEnd = lineStart + ((trajectoryList[i + 1] - trajectoryList[i]) * 0.6f);
					line.SetPosition(0, lineStart);
					line.SetPosition(1, lineEnd);
					line.enabled = true;
					line.gameObject.SetActive(true);

					float normal = Mathf.InverseLerp(0f, trajectoryCount, i);
					float lineAlpha = Mathf.Lerp(0.9f, 0.6f, Mathf.Sqrt(normal));
					Color lineColor = new Color(lineAlpha, lineAlpha, lineAlpha);
					float velocity = rb.velocity.magnitude * 0.05f;
					lineColor.g = Mathf.Clamp(lineColor.g - velocity, 0f, 1f);
					lineColor.b = Mathf.Clamp(lineColor.b - velocity, 0f, 1f);
					Color start = lineColor * 0.8f;
					start.a = 1f;
					line.startColor = start;
					line.endColor = lineColor;
				}
			}
			else if (line != null)
			{
				line.SetPosition(0, Vector3.zero);
				line.SetPosition(1, Vector3.zero);
				line.enabled = false;
				line.gameObject.SetActive(false);
				//Debug.Log("set inactive");
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
				lr.enabled = false;
				//lr.gameObject.SetActive(false);
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

	public void ClearPartialTrajectory(int startingFromIndex)
	{
		int numLines = lineList.Count;
		for(int i = startingFromIndex; i < numLines; i++)
		{
			if ((numLines > i) && (lineList[i] != null))
			{
				LineRenderer lr = lineList[i];
				lr.transform.localPosition = Vector3.zero;
				lr.enabled = false;
			}
		}
	}

	public void KillOrbit()
	{
		StopAllCoroutines();
		bUpdating = false;
		SetClampedLength(0);
		ClearTrajectory();
		ClearPartialTrajectory(0);
	}

	private IEnumerator delayStopCoroutine;
	public void DelayStop(float waitTime)
	{
		delayStopCoroutine = StopDelay(waitTime);
		StartCoroutine(delayStopCoroutine);
	}
	public IEnumerator StopDelay(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		SetUpdating(false);
		if (!bBackgroundUpdating)
		{
			bBackgroundUpdating = true;
			backgroundCoroutine = BackgroundUpdate(2f);
			StartCoroutine(backgroundCoroutine);
		}
	}

	private IEnumerator backgroundCoroutine;
	private IEnumerator BackgroundUpdate(float interval)
	{
		while (bBackgroundUpdating)
		{
			yield return new WaitForSeconds(interval);
			SimulateTrajectory(rb.velocity, -1f);
			RenderTrajectory();
		}
	}
}
