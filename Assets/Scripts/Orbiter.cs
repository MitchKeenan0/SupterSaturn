using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbiter : MonoBehaviour
{
	public float starMass = 1000f;

	private Rigidbody rb;

	void Start()
    {
		rb = GetComponent<Rigidbody>();
		if (!rb && transform.parent)
			rb = GetComponentInParent<Rigidbody>();

		float initV = Mathf.Sqrt(starMass / transform.position.magnitude);
		rb.velocity = transform.right * initV;
    }

	void FixedUpdate()
	{
		float r = Vector3.Magnitude(transform.position);
		float totalForce = -starMass / (r * r);
		Vector3 force = transform.position.normalized * totalForce;
		rb.AddForce(force);
	}
}
