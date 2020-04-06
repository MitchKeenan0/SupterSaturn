using System.Collections;
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
	private List<Vector3> raySpreadList;
	private List<Vector3> rayLerpList;
	private List<Vector3> tailLerpList;
	private float timeAtScan = 0f;
	private float timeElapsed = 0f;
	private bool bUpdating = false;
	private IEnumerator coneSpreadCoroutine;

    void Awake()
    {
		lineList = new List<RayLine>();
		raySpreadList = new List<Vector3>();
		rayLerpList = new List<Vector3>();
		tailLerpList = new List<Vector3>();

		for (int i = 0; i < rayLineCount; i++)
			SpawnRayLine();
	}

	void Update()
	{
		if (bUpdating)
		{
			timeElapsed += Time.deltaTime;

			int lineCount = lineList.Count;
			int spreadCount = raySpreadList.Count;
			if ((lineCount > 0) && (spreadCount > 0))
			{
				for(int i = 0; i < lineCount; i++)
				{
					if (((lineList.Count > i) && (lineList[i] != null))
						&& (rayLerpList.Count > i) && (rayLerpList[i] != null))
					{
						Vector3 origin = transform.position;
						RayLine line = lineList[i];

						if (timeElapsed < (scanLifetime * 0.8f))
						{
							if (!line.RayHit() && (line.RayExtent() < rayRange))
							{
								Vector3 lerp = rayLerpList[i];
								Vector3 spread = raySpreadList[i];
								lerp = Vector3.MoveTowards(lerp, spread, Time.deltaTime * raySpeed);
								rayLerpList[i] = lerp;
								line.SetRayLine(origin, lerp);
							}
							else
							{
								raySpreadList[i] = GetSpreadVector(i);
								rayLerpList[i] = transform.position;
								line.ResetRayLine();
							}
						}
						else
						{
							if (timeElapsed >= scanLifetime)
							{
								RetractLines();
								bUpdating = false;
								return;
							}
							else
							{
								Vector3 lerp = tailLerpList[i];
								Vector3 head = rayLerpList[i];
								lerp = Vector3.MoveTowards(lerp, head, Time.deltaTime * raySpeed);
								tailLerpList[i] = lerp;
								line.SetRayLine(lerp, Vector3.zero);
							}
						}
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
			timeAtScan = Time.time;
			ArrangeRayCone();
		}
	}

	void SpawnRayLine()
	{
		RayLine line = Instantiate(rayLinePrefab, transform);
		lineList.Add(line);
	}

	void ArrangeRayCone()
	{
		coneSpreadCoroutine = ConeSpread(0.06f);
		StartCoroutine(coneSpreadCoroutine);
	}

	private IEnumerator ConeSpread(float interval)
	{
		int index = 0;
		while (index < rayLineCount)
		{
			GetSpreadVector(index);
			index++;
			yield return new WaitForSeconds(interval);
		}
	}

	Vector3 GetSpreadVector(int index)
	{
		/// origin
		Vector3 lineStart = transform.position;

		if (rayLerpList.Count > index)
			rayLerpList[index] = lineStart;
		else
			rayLerpList.Add(lineStart);

		if (tailLerpList.Count > index)
			tailLerpList[index] = lineStart;
		else
			tailLerpList.Add(lineStart);

		/// direction
		Vector3 ray = transform.forward * rayRange;
		Vector3 spread = Random.onUnitSphere * raySpread;
		spread = Vector3.ProjectOnPlane(spread, ray);
		Vector3 lineEnd = lineStart + (ray + spread);

		if (raySpreadList.Count > index)
			raySpreadList[index] = lineEnd;
		else
			raySpreadList.Add(lineEnd);

		return lineEnd;
	}

	void RetractLines()
	{
		if (lineList == null)
			lineList = new List<RayLine>();
		if (raySpreadList == null)
			raySpreadList = new List<Vector3>();
		if (rayLerpList == null)
			rayLerpList = new List<Vector3>();

		int numLines = lineList.Count;
		for(int i = 0; i < numLines; i++)
		{
			RayLine rayLine = lineList[i];
			rayLine.SetRayLine(transform.position, transform.position);
			rayLine.ResetRayLine();
		}

		int numVectors = raySpreadList.Count;
		for(int i = 0; i < numVectors; i++)
			raySpreadList[i] = Vector3.zero;
		raySpreadList.Clear();

		int numLerps = rayLerpList.Count;
		for(int i = 0; i < numLerps; i++)
			rayLerpList[i] = Vector3.zero;
		rayLerpList.Clear();

		int numTails = tailLerpList.Count;
		for (int i = 0; i < numTails; i++)
			tailLerpList[i] = Vector3.zero;
		tailLerpList.Clear();
	}
}
