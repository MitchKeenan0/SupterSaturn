using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircleRenderer : MonoBehaviour
{
	// circle rendering
	[Range(0.1f, 100f)]
	public float circleRadius = 3.0f;
	[Range(3, 256)]
	public int numCircleSegments = 55;
	public float updateInterval = 0.1f;
	public bool bStartEnabled = true;
	public bool bAlwaysFaceCamera = false;

	public LineRenderer GetLineRenderer() { return circleLineRenderer; }
	private LineRenderer circleLineRenderer;
	private Camera cameraMain;
	private bool bAutoUpdating = false;

	private IEnumerator updateCoroutine;

	void Start()
    {
		cameraMain = Camera.main;
		circleLineRenderer = GetComponentInChildren<LineRenderer>();
		SetNumSegments(numCircleSegments);
		UpdateCircleRender();
		if (!bStartEnabled)
			circleLineRenderer.enabled = false;
	}

	private IEnumerator UpdateCircleTimer(float updateInterval)
	{
		while (true)
		{
			UpdateCircleRender();
			yield return new WaitForSeconds(updateInterval);
		}
	}

	public void StartAutoUpdate()
	{
		if (!bAutoUpdating)
		{
			bAutoUpdating = true;
			updateCoroutine = UpdateCircleTimer(updateInterval);
			StartCoroutine(updateCoroutine);
		}
	}

	public void EndAutoUpdate()
	{
		if (bAutoUpdating)
		{
			StopCoroutine(updateCoroutine);
			bAutoUpdating = false;
		}
	}

	public void Open()
	{
		circleLineRenderer.enabled = true;
	}

	public void Close()
	{
		EndAutoUpdate();
		circleLineRenderer.enabled = false;
	}

	public void SetPosition(Vector3 value)
	{
		transform.position = value;
	}

	public void SetRadius(float value)
	{
		circleRadius = value;
		UpdateCircleRender();
	}

	public void SetNumSegments(int value)
	{
		numCircleSegments = value;
		circleLineRenderer.positionCount = numCircleSegments + 1;
	}

	public void UpdateCircleRender()
	{
		if (circleLineRenderer != null)
		{
			circleLineRenderer.enabled = true;
			float deltaTheta = (float)(2.0 * Mathf.PI) / numCircleSegments;
			float theta = 0f;
			for (int i = 0; i < numCircleSegments + 1; i++)
			{
				float x = circleRadius * Mathf.Cos(theta);
				float z = circleRadius * Mathf.Sin(theta);
				Vector3 pos = new Vector3(x, 0, z);
				circleLineRenderer.SetPosition(i, pos);
				theta += deltaTheta;
			}

			if (bAlwaysFaceCamera)
				transform.LookAt(cameraMain.transform);
		}
	}
}
