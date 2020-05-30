using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftTrail : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private TrailRenderer tr;
	private Rigidbody rb;
	private IEnumerator updateCoroutine;

    void Start()
    {
		tr = GetComponent<TrailRenderer>();
		rb = GetComponentInParent<Rigidbody>();
		updateCoroutine = UpdateSpacecraftTrail(updateInterval);
		StartCoroutine(updateCoroutine);
    }

	private IEnumerator UpdateSpacecraftTrail(float interval)
	{
		while (true)
		{
			float spVelocity = rb.velocity.magnitude * 0.01f;
			tr.time = spVelocity;
			yield return new WaitForSeconds(interval);
		}
	}
}
