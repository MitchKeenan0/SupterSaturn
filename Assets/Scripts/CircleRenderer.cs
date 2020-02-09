using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
	// circle rendering
	[Range(0.1f, 100f)]
	public float circleRadius = 3.0f;
	[Range(3, 256)]
	public int numCircleSegments = 55;

	private LineRenderer circleLineRenderer;
	private Camera cameraMain;

	void Start()
    {
		cameraMain = Camera.main;
		circleLineRenderer = GetComponent<LineRenderer>();
		circleLineRenderer.positionCount = numCircleSegments + 1;
		UpdateCircleRender();
	}

	public void UpdateCircleRender()
	{
		transform.position = transform.parent.position;
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
	}
}
