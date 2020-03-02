using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCamera : MonoBehaviour
{
	public float distance = 100f;
	public Transform targetTransform;

	private CameraTargetFinder targetFinder;
	private TouchOrbit touchOrbit;
	private bool bActive = false;

    void Start()
    {
		targetFinder = GetComponent<CameraTargetFinder>();
		touchOrbit = GetComponent<TouchOrbit>();
		transform.position = Vector3.forward * -distance;
		this.enabled = bActive;
    }

    void Update()
    {
        if (bActive)
		{
			targetTransform = targetFinder.GetFirstTarget();
			if (targetTransform != null)
			{
				float targetDistance = targetTransform.position.magnitude;
				Vector3 cameraPosition = targetTransform.position.normalized * (distance + targetDistance);
				transform.position = cameraPosition;
				transform.LookAt(targetTransform);
			}
		}
    }

	public void SetTarget(Transform value)
	{
		targetTransform = value;
		SetActive(value != null);
	}

	public void SetActive(bool value)
	{
		bActive = value;
		this.enabled = value;
		touchOrbit.SetActive(!value);
	}
}
