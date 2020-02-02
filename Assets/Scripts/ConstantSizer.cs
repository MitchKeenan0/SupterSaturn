using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSizer : MonoBehaviour
{
	public float updateInterval = 0.1f;

	private Transform cameraTransform = null;
	private Vector3 naturalSize = Vector3.zero;
	private Vector3 naturalOffset = Vector3.zero;
	private float naturalWidth = 0;
	private IEnumerator updateCoroutine;

	private LineRenderer line;
	bool bPureTransform = true;
	bool bUpdating = false;

    void Start()
    {
		cameraTransform = Camera.main.transform;
		naturalSize = transform.localScale;
		naturalOffset = transform.localPosition;

		line = GetComponent<LineRenderer>();
		if (line != null)
		{
			naturalWidth = line.widthMultiplier;
			bPureTransform = false;
		}

		if (bPureTransform)
			StartUpdate();
	}

	public void StartUpdate()
	{
		if (!bUpdating)
		{
			StartCoroutine("UpdateSizeTimer", updateInterval);
			bUpdating = true;
		}
	}

	public void StopUpdate()
	{
		if (bUpdating)
		{
			StopAllCoroutines();
			bUpdating = false;
		}
	}

	private IEnumerator UpdateSizeTimer(float updateTime)
	{
		while(true)
		{
			yield return new WaitForSeconds(updateTime);
			UpdateSize();
		}
	}

	void UpdateSize()
	{
		float distToCamera = Vector3.Distance(transform.position, cameraTransform.position);

		if (bPureTransform)
		{
			transform.localScale = naturalSize * distToCamera;
			transform.localPosition = naturalOffset * distToCamera;
		}
		else if (line != null)
		{
			float firstPointDistance = Vector3.Distance(line.GetPosition(0), cameraTransform.position);
			float secondPointDistance = Vector3.Distance(line.GetPosition(1), cameraTransform.position);
			if (firstPointDistance <= secondPointDistance)
				distToCamera = firstPointDistance;
			else if (secondPointDistance < firstPointDistance)
				distToCamera = secondPointDistance;
			float lineWidth = naturalWidth * distToCamera * 0.15f;
			line.widthMultiplier = lineWidth;
		}
	}
}
