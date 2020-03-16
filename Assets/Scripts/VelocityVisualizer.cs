using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityVisualizer : MonoBehaviour
{
	public GameObject linePrefab;
	public int numLines = 30;

	private Rigidbody rb;
	private ObjectManager objectManager;
	private List<LineRenderer> lineRenderList;
	private List<Gravity> gravityList;
	
	void Awake()
	{
		lineRenderList = new List<LineRenderer>();
		gravityList = new List<Gravity>();
		objectManager = FindObjectOfType<ObjectManager>();
		if (objectManager != null)
			gravityList = objectManager.GetGravityList();
		rb = transform.parent.GetComponent<Rigidbody>();
		InitLines();
	}

	void Update()
	{
		if ((rb.velocity.magnitude > 0.5f) && (gravityList != null))
		{
			Vector3 previousPosition = transform.position;
			Vector3 simVelocity = rb.velocity;
			for (int i = 0; i < (numLines - 1); i++)
			{
				simVelocity += rb.velocity;
				simVelocity += AddGravities(simVelocity);
				SetLine(i, previousPosition, simVelocity);
				previousPosition = simVelocity;
			}
		}
	}

	Vector3 AddGravities(Vector3 velocityPosition)
	{
		Vector3 outVelocity = velocityPosition;
		if (gravityList != null)
		{
			foreach (Gravity g in gravityList)
				outVelocity += g.GetGravity(rb, velocityPosition, rb.mass);
		}
		return outVelocity;
	}

	void InitLines()
	{
		for (int i = 0; i < numLines; ++i)
		{
			GameObject newLine = Instantiate(linePrefab, transform.position, Quaternion.identity);
			newLine.transform.SetParent(transform);
			LineRenderer line = newLine.GetComponent<LineRenderer>();
			lineRenderList.Add(line);
			line.enabled = false;
			newLine.SetActive(false);
		}
	}

	public void SetLine(int lineIndex, Vector3 lineStart, Vector3 lineEnd)
	{
		if ((lineRenderList != null) && (lineRenderList.Count > 0))
		{
			LineRenderer line = lineRenderList[lineIndex];
			line.SetPosition(0, lineStart);
			line.SetPosition(1, lineEnd);
			line.gameObject.SetActive(true);
			line.enabled = true;

			LineMeasureHUD lineMeasure = transform.parent.GetComponentInChildren<LineMeasureHUD>();
			if (lineMeasure != null)
				lineMeasure.SetPosition(transform.parent.position);
		}
	}

	public void ClearLines()
	{
		foreach (LineRenderer line in lineRenderList)
		{
			line.enabled = false;
			line.gameObject.SetActive(false);
		}
	}

	public void SetRouteColor(Color value)
	{
		if (lineRenderList != null)
		{
			int numLines = lineRenderList.Count;
			for (int i = 0; i < numLines; i++)
			{
				if (lineRenderList[i] != null)
				{
					LineRenderer line = lineRenderList[i];
					Color newColor = value;
					float a = line.startColor.a;
					newColor.a = a;
					line.startColor = newColor;
					line.endColor = newColor;
				}
			}
		}
	}
}
