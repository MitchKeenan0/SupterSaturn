using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public bool bAutoOrbit = false;

	private CircleRenderer circleRenderer;
	private Autopilot autopilot;
	private Vector3 inputVector = Vector3.zero;
	private List<Vector3> trajectoryList;

	public List<Vector3> GetTrajectory() { return trajectoryList; }

	void Awake()
    {
		trajectoryList = new List<Vector3>();
		circleRenderer = GetComponentInChildren<CircleRenderer>();
    }

	void UpdateTrajectory()
	{
		trajectoryList.Clear();
		//
	}

	public void SetAutopilot(Autopilot ap)
	{
		autopilot = ap;
	}

	public void SetDirection(Vector3 direction)
	{
		if (bAutoOrbit)
		{
			inputVector = new Vector3(direction.x, direction.z, direction.y);
			transform.rotation = Quaternion.LookRotation(autopilot.gameObject.transform.position, direction);
		}
		else
		{
			//
		}
	}

	public void SetOrbitRange(float value)
	{
		if (bAutoOrbit)
		{
			circleRenderer.SetRadius(value);
		}
		else
		{
			//
		}
	}

	public void ModifyOrbitRange(float increment)
	{
		bool toeTest = (GetPoints()[0].magnitude > 1.6f);
		if (toeTest)
		{
			if (bAutoOrbit)
			{
				float nextRadius = circleRenderer.circleRadius + increment;
				circleRenderer.SetRadius(nextRadius);
				autopilot.SetRoute(GetPoints());
				autopilot.EnableMoveCommand(true);
			}
			else
			{
				//
			}
		}
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
				pointList.Add(transform.TransformPoint(vector));
		}
		return pointList;
	}
}
