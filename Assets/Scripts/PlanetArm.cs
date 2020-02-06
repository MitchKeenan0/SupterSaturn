﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetArm : MonoBehaviour
{
	public float armLength = 1.6f;
	public Transform planetTransform;

    void Start()
    {
		//transform.position = Vector3.zero;
    }

	public void SetLength(float value)
	{
		planetTransform.position = transform.position + (transform.forward * value * armLength);
	}
}