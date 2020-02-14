using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breather : MonoBehaviour
{
	public GameObject breathingObject;
	public float breathingSpeed = 1f;
	public float minimumSize = 0.1f;
	public float maximumSize = 1f;
	public bool bRandomStartDirection = true;
	public bool bIrregularScale = false;
	public float irregularity = 0.5f;

	private float controlSize = 0f;
	private float min = 0f;
	private float max = 0f;
	Vector3 targetSize = Vector3.one;

	void Start()
	{
		controlSize = breathingObject.transform.localScale.x;
		min = minimumSize * controlSize;
		max = maximumSize * controlSize;
		SetTargetMax();
		if (bRandomStartDirection && (Random.Range(0f, 1f) >= 0.5f))
			SetTargetMin();
	}

	private void Update()
	{
		if (breathingObject.transform.localScale.x <= (min + 1))
			SetTargetMax();
		else if (breathingObject.transform.localScale.x > (max - 1))
			SetTargetMin();

		breathingObject.transform.localScale = Vector3.LerpUnclamped(breathingObject.transform.localScale, targetSize, Time.deltaTime * breathingSpeed);
	}

	void SetTargetMax()
	{
		targetSize = Vector3.one * max;
		if (bIrregularScale)
		{
			targetSize.x += Random.Range(0f, controlSize * irregularity);
			targetSize.y += Random.Range(0f, controlSize * irregularity);
			targetSize.z += Random.Range(0f, controlSize * irregularity);
		}
	}

	public void SetTargetMin()
	{
		targetSize = Vector3.one * min;
		if (bIrregularScale)
		{
			targetSize.x -= Random.Range(0f, controlSize * irregularity);
			targetSize.y -= Random.Range(0f, controlSize * irregularity);
			targetSize.z -= Random.Range(0f, controlSize * irregularity);
		}
	}
}
