using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	public Camera cameraToLookAt;

	void Start()
	{
		cameraToLookAt = Camera.main;
	}

	void Update()
	{
		transform.LookAt(cameraToLookAt.transform);
	}
}
