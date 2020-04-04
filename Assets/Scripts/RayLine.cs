using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLine : MonoBehaviour
{
	private LineRenderer line;
	private Vector3 lineStart = Vector3.zero;
	private Vector3 lineEnd = Vector3.zero;
	private Vector3 hitPosition = Vector3.zero;
	private bool bHit = false;

	public bool RayHit() { return bHit; }
	public Vector3 GetHitPosition() { return hitPosition; }

    void Awake()
    {
		line = GetComponent<LineRenderer>();
    }

	void CheckRayHit()
	{
		Vector3 rayVector = lineEnd - lineStart;
		float distance = Vector3.Distance(lineStart, lineEnd);
		RaycastHit[] hits = Physics.RaycastAll(lineStart, rayVector, distance);
		if (hits.Length > 0)
		{
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				if (!hit.collider.isTrigger)
				{
					bHit = true;
					hitPosition = hit.point;
					///Debug.Log("scan ray hit " + hit.transform.gameObject.name);
				}
			}
		}
	}

	public void ResetRayLine()
	{
		bHit = false;
	}

	public void SetRayLine(Vector3 start, Vector3 end)
	{
		if (start != Vector3.zero)
		{
			line.SetPosition(0, start);
			lineStart = start;
		}
		if (end != Vector3.zero)
		{
			line.SetPosition(1, end);
			lineEnd = end;
		}
		if (!bHit)
			CheckRayHit();
	}
}
