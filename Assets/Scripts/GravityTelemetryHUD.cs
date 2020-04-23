using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTelemetryHUD : MonoBehaviour
{
	public GameObject telemetryLinePrefab;

	private GravitySystem gravitySystem;
	private List<Gravity> gravityList;
	private List<List<LineRenderer>> lineMasterList;
	private IEnumerator loadWaitCoroutine;

	void Awake()
	{
		gravityList = new List<Gravity>();
		lineMasterList = new List<List<LineRenderer>>();
		gravitySystem = FindObjectOfType<GravitySystem>();
	}

	void Start()
	{
		loadWaitCoroutine = LoadWait(0.6f);
		StartCoroutine(loadWaitCoroutine);
	}

	void Update()
	{
		if ((gravityList != null) && (gravityList.Count > 0))
		{
			int numGravities = gravityList.Count;
			for(int i = 0; i < numGravities; i++)
				UpdateTelemetry(i);
		}
		else
		{
			InitGravities();
		}
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		bool bGotAlready = ((gravityList != null) && (gravityList.Count > 0));
		if (!bGotAlready)
			InitGravities();
	}

	void InitGravities()
	{
		Gravity[] allGravities = FindObjectsOfType<Gravity>();
		foreach (Gravity g in allGravities)
		{
			if (!gravityList.Contains(g))
			{
				gravityList.Add(g);
				CreateLineList();
			}
		}
	}

	void UpdateTelemetry(int gravityIndex)
	{
		Gravity gravity = gravityList[gravityIndex];
		List<LineRenderer> lineList = new List<LineRenderer>();
		List<LineRenderer> usedLines = new List<LineRenderer>();
		if (gravityIndex < lineMasterList.Count)
			lineList = lineMasterList[gravityIndex];
		else
			lineList = CreateLineList();

		ClearLines(lineList);
		List<Rigidbody> gravityRbs = new List<Rigidbody>(gravity.GetRbList());
		int numInteractions = gravityRbs.Count;
		for (int i = 0; i < numInteractions; i++)
		{
			if (gravityRbs[i] != null)
			{
				Spacecraft sp = gravityRbs[i].gameObject.GetComponent<Spacecraft>();
				if (sp != null)
				{
					if ((sp.GetAgent() != null) && (sp.GetAgent().teamID == 0))
					{
						LineRenderer line = null;
						if (i < lineList.Count)
							line = lineList[i];
						else
							line = CreateTelemetryLine(lineList);

						usedLines.Add(line);
						line.SetPosition(0, sp.transform.position);
						line.SetPosition(1, gravity.bodyTransform.position);

						float lineDistance = 0.01f * Vector3.Distance(line.GetPosition(0), line.GetPosition(1));
						Color lineColor = line.startColor;
						lineColor.a = 1f / lineDistance;
						line.startColor = line.endColor = lineColor;

						if (!line.gameObject.activeInHierarchy)
							line.gameObject.SetActive(true);
					}
				}
			}
		}

		lineMasterList[gravityIndex] = lineList;
	}

	void ClearLines(List<LineRenderer> list)
	{
		for (int i = 0; i < list.Count; i++)
			list[i].gameObject.SetActive(false);
	}

	LineRenderer CreateTelemetryLine(List<LineRenderer> list)
	{
		GameObject obj = Instantiate(telemetryLinePrefab, transform);
		LineRenderer line = obj.GetComponent<LineRenderer>();
		list.Add(line);
		return line;
	}

	List<LineRenderer> CreateLineList()
	{
		List<LineRenderer> lineList = new List<LineRenderer>();
		lineMasterList.Add(lineList);
		return lineList;
	}

	public List<Gravity> GetGravitiesAffecting(Rigidbody rb)
	{
		List<Gravity> gravities = new List<Gravity>();
		if ((gravityList == null) || (gravityList.Count <= 0))
			InitGravities();
		foreach (Gravity grav in gravityList)
		{
			if ((grav.GetRbList() != null) && (grav.GetRbList().Count > 0))
			{
				if (grav.GetRbList().Contains(rb))
					gravities.Add(grav);
			}
		}
		return gravities;
	}
}
