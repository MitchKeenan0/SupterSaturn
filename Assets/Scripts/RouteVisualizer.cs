using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteVisualizer : MonoBehaviour
{
	public GameObject linePrefab;
	private Autopilot autoPilot;
	private List<LineRenderer> lineRenderList;
	private int numLines = 0;
	private int numClearedLines = 0;

	void Awake()
	{
		lineRenderList = new List<LineRenderer>();
	}

	void Start()
	{
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
			LineRenderer line = newLine.GetComponent<LineRenderer>();
			lineRenderList.Add(line);
			line.enabled = false;
			newLine.SetActive(false);
		}
	}

	public void SetLine(int lineIndex, Vector3 lineStart, Vector3 lineEnd)
	{
		if ((lineRenderList != null) && (lineRenderList.Count > lineIndex))
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

	public void ClearLine(int lineIndex)
	{
		if (lineIndex == -1)
		{
			numClearedLines = 0;
			if ((lineRenderList != null) && (lineRenderList.Count > 0))
			{
				foreach (LineRenderer line in lineRenderList)
				{
					line.enabled = false;
					line.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			int currentLine = lineIndex + numClearedLines;
			if (lineRenderList[currentLine] != null)
			{
				LineRenderer line = lineRenderList[currentLine];
				if (line.enabled)
				{
					line.enabled = false;
					line.gameObject.SetActive(false);
					numClearedLines++;
				}
			}
		}
	}

	public void SetRouteColor(Color value)
	{
		if (lineRenderList != null)
		{
			int numLines = lineRenderList.Count;
			for(int i = 0; i < numLines; i++)
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
