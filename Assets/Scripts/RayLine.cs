using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLine : MonoBehaviour
{
	private LineRenderer line;
	private Transform originTransform;
	private Vector3 lineStart = Vector3.zero;
	private Vector3 lineEnd = Vector3.zero;
	private Vector3 hitPosition = Vector3.zero;
	private Vector3 rayVector = Vector3.zero;
	private float raySpeed = 1f;
	private bool bHit = false;
	private bool bEnabled = false;
	private bool bFinished = false;
	private int scanVertsHit = 0;

	public bool IsEnabled() { return bEnabled; }
	public bool RayHit() { return bHit; }
	public bool LineFinished() { return bHit && bFinished; }
	public float RayExtent() { return Vector3.Distance(lineStart, lineEnd); }
	public Vector3 GetHitPosition() { return hitPosition; }
	public int GetScanHits() { return scanVertsHit; }

    void Awake()
    {
		line = GetComponent<LineRenderer>();
    }

	void CheckRayHit()
	{
		Vector3 rayVector = (lineEnd - lineStart);
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

					Planet planet = hit.transform.gameObject.GetComponent<Planet>();
					if (planet == null)
						planet = hit.transform.GetComponentInChildren<Planet>();
					if (planet != null)
					{
						int hitVerts = planet.GetScanned(hit.point, 25f, Color.cyan);
						if (hitVerts > 0)
							scanVertsHit += hitVerts;
					}
				}
			}
		}
	}

	public void SetLineFromTransform(Transform origin, Vector3 lineVector, float speed)
	{
		ResetRayLine();
		originTransform = origin;
		rayVector = lineVector;
		raySpeed = speed;
		transform.rotation = Quaternion.LookRotation(rayVector, Vector3.up);
	}

	public void UpdateRayLine()
	{
		if (!bHit && (RayExtent() < rayVector.magnitude))
		{
			lineEnd = Vector3.MoveTowards(lineEnd, rayVector, Time.deltaTime * raySpeed);
			lineStart = originTransform.position;
		}
		else
		{
			lineStart = Vector3.MoveTowards(lineStart, lineEnd, Time.deltaTime * raySpeed);
			if (Vector3.Distance(lineStart, lineEnd) <= 1f)
				bFinished = true;
		}
		SetRayLine(lineStart, lineEnd);
	}

	public void ResetRayLine()
	{
		bHit = false;
		bFinished = false;
		scanVertsHit = 0;
		lineStart = lineEnd = transform.position;
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

	public void Finishing()
	{
		bHit = true;
	}

	public void SetEnabled(bool value)
	{
		bEnabled = value;
	}
}
