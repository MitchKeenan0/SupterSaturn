﻿using UnityEngine;

public class Rotator : MonoBehaviour
{
	[Range(-1.0f, 1.0f)]
	public float xForceDirection = 0.0f;
	[Range(-1.0f, 1.0f)]
	public float yForceDirection = 0.0f;
	[Range(-1.0f, 1.0f)]
	public float zForceDirection = 0.0f;

	public float speedMultiplier = 1;

	public bool bRandomRotationStart = false;
	public bool bRandomSpeed = false;

	public bool worldPivot = false;

	private Space spacePivot = Space.Self;
	
	void Start()
	{
		if (worldPivot) spacePivot = Space.World;
		if (bRandomRotationStart)
		{
			float rand1 = Random.Range(0, 360);
			float rand2 = Random.Range(0, 360);
			float rand3 = Random.Range(0, 360);
			float x = Mathf.Clamp(xForceDirection * 10000f, -1f, 1f);
			float y = Mathf.Clamp(yForceDirection * 10000f, -1f, 1f);
			float z = Mathf.Clamp(zForceDirection * 10000f, -1f, 1f);
			transform.Rotate((x * rand1), (y * rand2), (z * rand3), spacePivot);
		}

		if (bRandomSpeed)
			speedMultiplier = Random.Range(speedMultiplier * 0.3f, speedMultiplier);
	}

	void Update()
	{
		float deltaSpeed = Time.deltaTime * speedMultiplier;
		transform.Rotate(xForceDirection * deltaSpeed
							, yForceDirection * deltaSpeed
							, zForceDirection * deltaSpeed
							, spacePivot);
	}
}