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
		planetTransform.position = transform.position + (transform.forward * value * armLength);
		if (offsetRotationScale != 0f)
		{
			Vector3 offsetEuler = Vector3.zero;
			offsetEuler.x = Random.Range(-offsetRotationScale, offsetRotationScale);
			offsetEuler.y = Random.Range(-offsetRotationScale, offsetRotationScale);
			offsetEuler.z = Random.Range(-offsetRotationScale, offsetRotationScale);
			transform.rotation = Quaternion.Euler(offsetEuler);
			if (circle != null)
			{
				circle.SetRadius(value * 2);
				circle.UpdateCircleRender();
			}
		}
	}
}
