using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	private CircleRenderer circleRenderer;
	private Autopilot autopilot;

    void Start()
    {
		circleRenderer = GetComponentInChildren<CircleRenderer>();
    }

	public void SetAutopilot(Autopilot ap)
	{
		autopilot = ap;
	}

	public void SetDirection(Vector3 direction)
	{
		Vector3 directionEuler = new Vector3(direction.x, direction.z, direction.y);
		transform.rotation = Quaternion.Euler(directionEuler);
	}

	public void SetOrbitRange(float value)
	{
		circleRenderer.SetRadius(value);
	}

	public void ModifyOrbitRange(float increment)
	{
		float nextRadius = circleRenderer.circleRadius + increment;
		circleRenderer.SetRadius(nextRadius);
		autopilot.SetRoute(GetPoints());
		autopilot.EnableMoveCommand(true);
	}

	public List<Vector3> GetPoints()
	{
		List<Vector3> pointList = new List<Vector3>();
		LineRenderer lineRenderer = circleRenderer.GetLineRenderer();
		int numPositions = lineRenderer.positionCount;
		Vector3[] vArray = new Vector3[numPositions];
		lineRenderer.GetPositions(vArray);
		if (vArray.Length > 0)
		{
			foreach(Vector3 vector in vArray)
				pointList.Add(transform.InverseTransformPoint(vector));
		}
		return pointList;
	}
}
