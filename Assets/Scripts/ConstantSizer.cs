using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSizer : MonoBehaviour
{
	public float updateInterval = 0.1f;
	public float scaleFactor = 1f;

	private Transform cameraTransform = null;
	private Vector3 naturalSize = Vector3.zero;
	private Vector3 naturalOffset = Vector3.zero;
	private float naturalWidth = 0;

	private LineRenderer line;
	bool bPureTransform = true;

    void Start()
    {
		cameraTransform = Camera.main.transform;
		naturalSize = transform.localScale;
		naturalOffset = transform.localPosition;

		line = GetComponent<LineRenderer>();
		if (!line)
			line = GetComponentInChildren<LineRenderer>();
		if (line != null)
		{
			naturalWidth = line.widthMultiplier;
			bPureTransform = false;
		}
	}

	void Update()
	{
		UpdateSize();
	}

	void UpdateSize()
	{
		Vector3 cameraPosition = cameraTransform.position;
		float distToCamera = Vector3.Distance(transform.position, cameraPosition);
		distToCamera *= scaleFactor;
		if (bPureTransform)
		{
			transform.localScale = naturalSize * distToCamera;
			transform.localPosition = naturalOffset * distToCamera;
		}
		else if (line != null)
		{
			float firstPointDistance = Vector3.Distance(line.GetPosition(0), cameraPosition);
			float secondPointDistance = Vector3.Distance(line.GetPosition(1), cameraPosition);
			if (firstPointDistance <= secondPointDistance)
				distToCamera = firstPointDistance;
			else if (secondPointDistance < firstPointDistance)
				distToCamera = secondPointDistance;

			line.widthMultiplier = naturalWidth * distToCamera * scaleFactor;
		}
	}
}
