﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteVisualizer : MonoBehaviour
{
	public GameObject linePrefab;
	private Autopilot autoPilot;
	private List<LineRenderer> lineRenderList;
	private int numLines = 0;

	void Start()
	{
		lineRenderList = new List<LineRenderer>();
		autoPilot = GetComponentInParent<Autopilot>();
		InitLines();
	}

	void InitLines()
	{
		numLines = autoPilot.routeVectorPoints;
		for (int i = 0; i < numLines; ++i)
		{
			GameObject newLine = Instantiate(linePrefab, transform.position, Quaternion.identity);
			newLine.transform.SetParent(transform);
			lineRenderList.Add(newLine.GetComponent<LineRenderer>());
			newLine.GetComponent<LineRenderer>().enabled = false;
		}
	}

	public void SetLine(int lineIndex, Vector3 lineStart, Vector3 lineEnd)
	{
		LineRenderer line = lineRenderList[lineIndex];
		line.SetPosition(0, lineStart);
		line.SetPosition(1, lineEnd);
		line.enabled = true;
		line.gameObject.SetActive(true);
	}

	public void ClearLine(int lineIndex)
	{
		if (lineIndex == -1)
		{
			foreach (LineRenderer line in lineRenderList)
				line.enabled = false;
		}
		else
		{
			if (lineRenderList[lineIndex] != null)
			{
				LineRenderer line = lineRenderList[lineIndex];
				line.enabled = false;
				line.gameObject.SetActive(false);
				Debug.Log("Cleared line " + lineIndex);
			}
		}
	}
}
