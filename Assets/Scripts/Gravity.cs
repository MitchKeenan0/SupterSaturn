using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
	public float radius = 3f;
	public float strength = 0.1f;
	public float updateInterval = 0.1f;
	private Rigidbody[] rbs;

	private IEnumerator surroundingCheckCoroutine;

    void Start()
    {
		rbs = FindObjectsOfType<Rigidbody>();
		surroundingCheckCoroutine = SurroundingCheck(updateInterval);
    }

    void FixedUpdate()
    {
		if (rbs.Length > 0){
			foreach (Rigidbody r in rbs){
				if (r != null && !r.isKinematic)
				{
					Vector3 g = GetGravity(r);
					if (g != Vector3.zero)
						r.AddForce(g);
				}
			}
		}
    }

	public Vector3 GetGravity(Rigidbody r)
	{
		Vector3 gravity = Vector3.zero;
		if (r != null && !r.isKinematic)
		{
			Vector3 toCenter = transform.position - r.position;
			if (toCenter.magnitude <= radius)
			{
				float G = (1f / toCenter.magnitude) * r.mass;
				gravity = toCenter.normalized * G * strength;
				r.AddForce(gravity);
			}
		}
		return gravity;
	}

	private IEnumerator SurroundingCheck(float intervalTime)
	{
		while (true)
		{
			Collider[] nears = Physics.OverlapSphere(transform.position, radius);
			int numNears = nears.Length;
			for (int i = 0; i < numNears; i++)
			{
				if (nears[i] != null)
				{
					Rigidbody r = nears[i].GetComponent<Rigidbody>();
					if (r != null)
					{
						rbs[i] = r;
					}
				}
			}

			yield return new WaitForSeconds(intervalTime);
		}
	}
}
