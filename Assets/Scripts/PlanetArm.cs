using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetArm : MonoBehaviour
{
	public float armLength = 1.6f;
	public Transform planetTransform;
	public float offsetRotationScale = 0.1f;

	private CircleRenderer circle;

    void Awake()
    {
		circle = gameObject.GetComponentInChildren<CircleRenderer>();
    }

	public void SetLength(float value)
	{
		planetTransform.localPosition = (transform.root.forward * value * armLength);
		if (circle != null)
		{
			circle.SetRadius(value * armLength);
			circle.UpdateCircleRender();
		}
	}
}
