using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
	public Transform bodyTransform;
	public float radius = 3f;
	public float strength = 0.1f;

	private List<Rigidbody> rbList;
	private GravityTelemetryHUD telemetry;
	private PlanetArm planetArm;
	private IEnumerator surroundingCheckCoroutine;

	public List<Rigidbody> GetRbList() { return rbList; }
	public void SetRbList(List<Rigidbody> list) { rbList = list; }

	void Awake()
	{
		rbList = new List<Rigidbody>();
	}

    void Start()
    {
		planetArm = FindObjectOfType<PlanetArm>();
		telemetry = FindObjectOfType<GravityTelemetryHUD>();
		if (!bodyTransform)
		{
			if (planetArm != null)
				bodyTransform = planetArm.planetTransform;
			else
				bodyTransform = transform;
		}
    }

    void FixedUpdate()
    {
		int numRbs = rbList.Count;
		if (numRbs > 0)
		{
			for(int i = 0; i < numRbs; i++)
			{
				Rigidbody r = rbList[i];
				if ((r != null) && (!r.isKinematic))
				{
					Vector3 g = GetGravity(r);
					if (g != Vector3.zero)
						r.AddForce(g * Time.fixedDeltaTime);
				}
			}
		}
    }

	public Vector3 GetGravity(Rigidbody r)
	{
		Vector3 gravity = Vector3.zero;
		if ((r != null) && !r.isKinematic)
		{
			Debug.DrawLine(r.transform.position, bodyTransform.position, Color.magenta);
			Vector3 toCenter = bodyTransform.position - r.position;
			if (toCenter.magnitude <= radius)
			{
				float G = (1f / toCenter.magnitude) * r.mass * strength;
				gravity = toCenter.normalized * G;
				r.AddForce(gravity);
			}
		}
		return gravity;
	}
}
