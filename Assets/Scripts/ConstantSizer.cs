using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSizer : MonoBehaviour
{
	public float updateInterval = 0.1f;

	private Transform cameraTransform;
	private Vector3 naturalSize;
	private Vector3 naturalOffset;
	private IEnumerator updateCoroutine;

    void Start()
    {
		cameraTransform = Camera.main.transform;
		naturalSize = transform.localScale;
		naturalOffset = transform.localPosition;
		updateCoroutine = UpdateSizeTimer(updateInterval);
		StartCoroutine(updateCoroutine);
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
		transform.localScale = naturalSize * distToCamera;
		transform.localPosition = naturalOffset * distToCamera;
	}
}
