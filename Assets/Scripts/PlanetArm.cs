using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetArm : MonoBehaviour
{
	public float armLength = 1.6f;
	public Transform planetTransform;

    void Start()
    {
		transform.position = Vector3.zero;
    }

	public void SetLength(int i)
	{
		planetTransform.localPosition = Vector3.forward * i * armLength;
	}
}
