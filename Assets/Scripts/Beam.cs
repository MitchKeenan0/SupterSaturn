using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
	public LineRenderer line;
	public Transform owningTransform;
	public Transform targetTransform;
	private Vector3 lerpEndPosition = Vector3.zero;

    void Awake()
    {
		line = GetComponent<LineRenderer>();
    }

	public virtual void SetEnabled(bool value)
	{
		if (!line)
			line = GetComponent<LineRenderer>();
		if (line != null)
			line.enabled = value;
		if (!value)
			targetTransform = null;
	}

	public virtual void SetLinePositions(Vector3 start, Vector3 end, float speed)
	{
		if (!line)
			line = GetComponent<LineRenderer>();
		if (line != null)
		{
			line.SetPosition(0, start);
			if (speed == 0f)
				lerpEndPosition = end;
			else
				lerpEndPosition = Vector3.MoveTowards(lerpEndPosition, end, Time.deltaTime * speed);
			line.SetPosition(1, lerpEndPosition);
		}
	}

	public virtual void UpdateBeam()
	{
		///
	}

	public virtual void SetOwner(Transform value)
	{
		owningTransform = value;
	}

	public virtual void SetTarget(Transform value)
	{
		targetTransform = value;
	}
}
