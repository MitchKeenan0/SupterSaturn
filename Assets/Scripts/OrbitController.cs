﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public LineRenderer trajectoryLinePrefab;
	public int maxLineCount = 250;
	public float updateInterval = 0.1f;
	public float backgroundUpdateInterval = 2f;
	public Material lineMaterial;
	public Material checkpointMaterial;
	public Material collisionMaterial;

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
	private bool bBackgroundUpdating = false;
	private bool bRenderFlip = false;
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

		bBackgroundUpdating = true;
		backgroundCoroutine = BackgroundUpdate(backgroundUpdateInterval);
		StartCoroutine(backgroundCoroutine);
	}

	public void SetBurnDuration(float value)
	{
		burnDuration = value;
	}

	void SimulateTrajectory(Vector3 heading, float duration)
	{
		ClearTrajectory();

		Spacecraft spacecraft = autopilot.gameObject.GetComponent<Spacecraft>();
		duration = Mathf.Clamp(rb.velocity.magnitude * 0.6f, 0.001f, 30f);

		float simulationTime = 0f;
		float deltaTime = Time.fixedDeltaTime;
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
			int numGravs = gravityList.Count;
			for (int i = 0; i < numGravs; i++)
			{
				Gravity gr = gravityList[i];
				trajecto += (gr.GetGravity(rb, currentPosition, rb.mass));
			}

			velocity = currentPosition + trajecto;
			trajectoryList.Add(velocity);
			deltaVelocity = trajecto;
			currentPosition = velocity;
			simulationTime += deltaTime;
		}
	}

	void RenderTrajectory()
	{
		int trajectoryCount = trajectoryList.Count;
		for (int i = 0; i < maxLineCount; i++)
		{
			//bRenderFlip = !bRenderFlip;
			//if (bRenderFlip)
			//	continue;
			LineRenderer line = null;
			if ((i < lineList.Count) && (lineList[i] != null))
			{
				line = lineList[i];
			}

			if ((i < trajectoryList.Count) && (i < trajectoryClampedLength) && (line != null))
			{
				Vector3 lineStart = trajectoryList[i];
				if (trajectoryList.Count > (i + 1))
				{
					Vector3 lineEnd = lineStart + ((trajectoryList[i + 1] - trajectoryList[i]) * 0.9f);

					Vector3 origin = lineList[0].GetPosition(0);
					float thresholdDistance = 5f;
					bool innerThresholdMasked = ((Vector3.Distance(lineStart, origin) < thresholdDistance) 
												|| (Vector3.Distance(lineEnd, origin) < thresholdDistance));
					if (innerThresholdMasked)
					{
						line.enabled = false;
					}
					else
					{
						line.SetPosition(0, lineStart);
						line.SetPosition(1, lineEnd);
						line.enabled = true;
						line.gameObject.SetActive(true);

						/// diminishing color
						float normal = Mathf.InverseLerp(0f, trajectoryCount, i);
						float lineAlpha = Mathf.Lerp(1f, 0.1f, Mathf.Sqrt(normal));
						Color lineColor = new Color(lineAlpha, lineAlpha, lineAlpha);
						float velocity = rb.velocity.magnitude * 0.005f;
						lineColor.g = Mathf.Clamp(lineColor.g - velocity, 0f, 0.5f);
						lineColor.b = Mathf.Clamp(lineColor.b - velocity, 0f, 0.5f);
						Color start = lineColor * 0.8f;
						start.a = 1f;
						line.startColor = start;
						line.endColor = lineColor;
						//line.widthMultiplier = lineAlpha * 0.1f;
					}
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

	// 0=line 1=checkpoint 2=collision
	public void SetTrajectoryColor(Color value, int materialIndex)
	{
		foreach(LineRenderer lr in lineList)
		{
			lr.startColor = lr.endColor = value;
			Material lrMat = null;
			switch(materialIndex)
			{
				case 0: lrMat = lineMaterial;
					break;
				case 1: lrMat = checkpointMaterial;
					break;
				case 2: lrMat = collisionMaterial;
					break;
				default:
					break;
			}
			lr.material = lrMat;
		}
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
		SetClampedLength(0);
		ClearTrajectory();
		ClearPartialTrajectory(0);
	}

	public void SetBackgroundUpdateInterval(float value)
	{
		StopAllCoroutines();
		backgroundCoroutine = BackgroundUpdate(value);
		StartCoroutine(backgroundCoroutine);
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
