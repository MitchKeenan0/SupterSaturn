using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
	public float radius = 3f;
	public float strength = 0.1f;
	public float updateInterval = 0.1f;

	private List<Rigidbody> rbList;
	private GravityTelemetryHUD telemetry;
	private IEnumerator surroundingCheckCoroutine;

	public List<Rigidbody> GetRBList() { return rbList; }

    void Start()
    {
		rbList = new List<Rigidbody>();
		telemetry = FindObjectOfType<GravityTelemetryHUD>();
		surroundingCheckCoroutine = SurroundingCheck(updateInterval);
		StartCoroutine(surroundingCheckCoroutine);
    }

    void FixedUpdate()
    {
		if (rbList.Count > 0){
			foreach (Rigidbody r in rbList)
			{
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
			yield return new WaitForSeconds(intervalTime);

			Collider[] nears = Physics.OverlapSphere(transform.position, radius);
			int numNears = nears.Length;
			int numListed = rbList.Count;
			for (int i = 0; i < numNears; i++){
				if (nears[i] != null){
					Rigidbody r = nears[i].GetComponent<Rigidbody>();
					if (r != null){
						if (rbList.Contains(r))
						{
							continue;
						}
						else
						{
							rbList.Add(r);
							numListed++;
						}
					}
				}
			}

			// check over for objects beyond reach
			int numRbs = rbList.Count;
			if (numRbs > 0)
			{
				Vector3 myPos = transform.position;
				for(int i = 0; i < numRbs; i++)
				{
					if ((i < rbList.Count) && (rbList[i] != null))
					{
						float distance = Vector3.Distance(rbList[i].transform.position, myPos);
						if (distance >= radius)
							rbList.RemoveAt(i);
					}
				}
			}
		}
	}
}
