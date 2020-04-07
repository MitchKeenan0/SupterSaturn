using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitonBeam : MonoBehaviour
{
	private LineRenderer line;
	private Transform owningTransform;
	private Transform targetTransform;

    void Awake()
    {
		line = GetComponent<LineRenderer>();
    }

	public void SetEnabled(bool value)
	{
		if (!line)
			line = GetComponent<LineRenderer>();
		line.enabled = value;
	}

	public void SetLinePositions(Vector3 start, Vector3 end)
	{
		if (!line)
			line = GetComponent<LineRenderer>();
		line.SetPosition(0, start);
		line.SetPosition(1, end);
	}

	public void UpdateGravitonBeam()
	{

	}

	public void SetOwner(Transform value)
	{
		owningTransform = value;
	}

	public void SetTarget(Transform value)
	{
		targetTransform = value;
	}
}
