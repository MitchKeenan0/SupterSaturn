using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTelemetryHUD : MonoBehaviour
{
	public GameObject telemetryLinePrefab;

	private List<Gravity> gravityList;
	private List<LineRenderer> lineList;
	private IEnumerator loadWaitCoroutine;

	void Awake()
	{
		gravityList = new List<Gravity>();
		lineList = new List<LineRenderer>();
	}

	void Start()
	{
		loadWaitCoroutine = LoadWait(0.5f);
		StartCoroutine(loadWaitCoroutine);
	}

	void Update()
	{
		if ((gravityList != null) && (gravityList.Count > 0))
		{
			foreach (Gravity g in gravityList)
				UpdateTelemetry(g);
		}
	}

	void UpdateTelemetry(Gravity gravity)
	{
		List<Rigidbody> gravityRbs = new List<Rigidbody>(gravity.GetRBList());
		int numInteractions = gravityRbs.Count;
		ClearLines();
		for(int i = 0; i < numInteractions; i++)
		{
			if (gravityRbs[i] != null)
			{
				LineRenderer line = null;
				if (i < lineList.Count)
					line = lineList[i];
				else
					line = CreateTelemetryLine();
				Spacecraft sp = gravityRbs[i].gameObject.GetComponent<Spacecraft>();
				if (sp != null)
				{
					if (!line.gameObject.activeInHierarchy)
						line.gameObject.SetActive(true);
					line.SetPosition(0, sp.transform.position);
					line.SetPosition(1, gravity.transform.position);
				}
			}
		}
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitGravities();
		InitLines();
	}

	void InitGravities()
	{
		Gravity[] allGravities = FindObjectsOfType<Gravity>();
		foreach (Gravity g in allGravities)
			gravityList.Add(g);
	}

	void InitLines()
	{
		for (int i = 0; i < 10; i++)
			CreateTelemetryLine();
	}

	void ClearLines()
	{
		for (int i = 0; i < lineList.Count; i++)
			lineList[i].gameObject.SetActive(false);
	}

	LineRenderer CreateTelemetryLine()
	{
		GameObject obj = Instantiate(telemetryLinePrefab, transform);
		obj.SetActive(false);
		LineRenderer line = obj.GetComponent<LineRenderer>();
		lineList.Add(line);
		return line;
	}
}
