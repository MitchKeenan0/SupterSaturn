﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanRay : Scan
{
	public RayLine rayLinePrefab;
	public int rayLineCount = 25;
	public float rayRange = 1000f;
	public float raySpread = 100f;
	public float raySpeed = 1000f;

	private List<RayLine> lineList;
	private float timeElapsed = 0f;
	private bool bUpdating = false;
	private bool bArrangingCone = false;
	private IEnumerator coneSpreadCoroutine;

    void Awake()
    {
		lineList = new List<RayLine>();
		for (int i = 0; i < rayLineCount; i++)
			SpawnRayLine();
	}

	void Update()
	{
		if (bUpdating)
		{
			timeElapsed += Time.deltaTime;

			int lineCount = lineList.Count;
			for (int i = 0; i < lineCount; i++)
			{
				if ((lineList.Count > i) && (lineList[i] != null))
				{
					RayLine line = lineList[i];
					if (line.IsEnabled())
					{
						if (timeElapsed >= (scanLifetime * 0.5f))
							line.Finishing();
						else if (line.LineFinished())
							GetSpreadVector(i);

						line.UpdateRayLine();
					}
				}
			}
		}
	}

	public override void InitScan(Transform owningScanner, Quaternion rotation, float lifeTime)
	{
		base.InitScan(owningScanner, rotation, lifeTime);

		if (owningScanner != null)
			RetractLines();
	}

	public override void SetEnabled(bool value)
	{
		base.SetEnabled(value);
		bUpdating = value;
		if (bUpdating)
		{
			timeElapsed = 0f;
			ArrangeRayCone();
		}
		else 
		{
			if (bArrangingCone)
				StopCoroutine(coneSpreadCoroutine);
			RetractLines();
		}
	}

	void SpawnRayLine()
	{
		RayLine line = Instantiate(rayLinePrefab, transform);
		lineList.Add(line);
	}

	void ArrangeRayCone()
	{
		coneSpreadCoroutine = ConeSpread(0.05f);
		StartCoroutine(coneSpreadCoroutine);
	}

	private IEnumerator ConeSpread(float interval)
	{
		bArrangingCone = true;
		int i = 0;
		while (i < rayLineCount)
		{
			GetSpreadVector(i);
			i++;
			yield return new WaitForSeconds(interval);
		}
		bArrangingCone = false;
	}

	Vector3 GetSpreadVector(int index)
	{
		/// origin
		Vector3 lineStart = transform.position;

		/// direction
		Vector3 ray = transform.forward * rayRange;
		Vector3 spread = Random.onUnitSphere * raySpread;
		spread = Vector3.ProjectOnPlane(spread, ray);
		Vector3 lineEnd = lineStart + (ray + spread);

		if ((lineList.Count > index) && (lineList[index] != null))
		{
			RayLine line = lineList[index];
			line.SetLineFromTransform(transform, lineEnd, raySpeed);
			line.SetEnabled(true);
		}

		return lineEnd;
	}

	void RetractLines()
	{
		int numLines = lineList.Count;
		for(int i = 0; i < numLines; i++)
		{
			RayLine rayLine = lineList[i];
			rayLine.SetRayLine(transform.position, transform.position);
			rayLine.ResetRayLine();
			rayLine.SetEnabled(false);
		}
	}
}