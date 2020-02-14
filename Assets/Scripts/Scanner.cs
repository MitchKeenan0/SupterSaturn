using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
	public float scanInterval = 1f;
	public float intervalDeviation = 0.1f;
	public float scanSpeed = 1f;
	public float maxRadius = 1f;
	public float updateInterval = 0.1f;
	public float scanRecoveryPeriod = 0.5f;

	private Collider[] hits;
	private List<GameObject> targetList;
	private GameObject scanRender;
	private MeshRenderer meshRenderer;
	private Spacecraft mySpacecraft;
	private TargetPredictionHUD predictionHud;
	private IEnumerator scanCoroutine;
	private IEnumerator updateCoroutine;
	private IEnumerator recoveryCoroutine;
	private float currentRadius = 0f;
	private float timeAtScan = 0f;
	private float originalAlpha = 0;

	public GameObject[] GetTargets() { return targetList.ToArray(); }

	void Start()
    {
		targetList = new List<GameObject>();
		mySpacecraft = GetComponentInParent<Spacecraft>();
		predictionHud = FindObjectOfType<TargetPredictionHUD>();

		meshRenderer = GetComponentInChildren<MeshRenderer>();
		scanRender = meshRenderer.gameObject;
		originalAlpha = meshRenderer.material.color.a;
		meshRenderer.enabled = false;

		scanCoroutine = LoopingScanLaunch(scanInterval + scanRecoveryPeriod + Random.Range(-intervalDeviation, intervalDeviation));
		StartCoroutine(scanCoroutine);
	}

	private IEnumerator LoopingScanLaunch(float waitTime)
	{
		while (true)
		{
			LaunchScan();
			yield return new WaitForSeconds(waitTime);
		}
	}

	void LaunchScan()
	{
		ClearTargets();
		currentRadius = 0f;
		timeAtScan = Time.time;

		updateCoroutine = UpdateScan(updateInterval);
		StartCoroutine(updateCoroutine);
	}

	void UpdateScan()
	{
		if (mySpacecraft.GetAgent().teamID == 0)
		{
			UpdateScanRender();
			if (!meshRenderer.enabled)
				meshRenderer.enabled = true;
		}

		currentRadius += (scanSpeed * Time.deltaTime);
		bool bScanFinished = (Time.time - timeAtScan) >= scanInterval;
		if (bScanFinished)
		{
			ClearTargets();
			StopCoroutine(updateCoroutine);
			return;
		}

		hits = Physics.OverlapSphere(transform.position, currentRadius);
		int numHits = hits.Length;
		if (numHits > 0){
			for (int i = 0; i < numHits; i++)
			{
				Collider hit = hits[i];
				if (hit.transform != gameObject.transform)
				{
					Spacecraft sp = hit.gameObject.GetComponent<Spacecraft>();
					if ((sp != null) 
						&& sp.IsAlive()
						&& (sp.GetAgent().teamID != mySpacecraft.GetAgent().teamID)
						&& mySpacecraft.GetAgent().LineOfSight(sp.transform.position, sp.transform)
						&& (Vector3.Distance(sp.transform.position, transform.position) <= maxRadius))
					{
						targetList.Add(hit.gameObject);
						sp.GetComponent<Spacecraft>().AddMarkValue(1);
					}
				}
			}
		}
	}

	void UpdateScanRender()
	{
		float safeRadius = Mathf.Clamp(currentRadius, 0.01f, maxRadius);
		scanRender.transform.localScale = Vector3.one * safeRadius * 2f;
		scanRender.transform.rotation = Quaternion.identity;

		Color scanColor = meshRenderer.material.color;
		float percentOfLifetime = 1f - ((Time.time - timeAtScan) / scanInterval);
		scanColor.a = Mathf.Clamp(originalAlpha * percentOfLifetime, 0f, 1f);
		meshRenderer.material.color = scanColor;
	}

	void ClearTargets()
	{
		int numTargets = targetList.Count;
		if (numTargets > 0)
		{
			// looping thru each target..
			for(int i = 0; i < numTargets; i++)
			{
				if ((targetList.Count > i) && (targetList[i] != null))
				{
					GameObject c = targetList[i];

					// optionally keep predictions unless theyre gone
					Prediction targetPrediction = c.GetComponent<Prediction>();
					if (targetPrediction != null)
					{
						if ((targetPrediction.spacecraft == null) || (!targetPrediction.spacecraft.IsAlive()))
							targetList.Remove(targetPrediction.gameObject);
					}
					
					// create prediction and lose contact
					else if (c.GetComponent<Spacecraft>())
					{
						Spacecraft sp = c.GetComponent<Spacecraft>();
						sp.AddMarkValue(-1);
						if (mySpacecraft.GetAgent().teamID == 0)
							predictionHud.SetPrediction(sp, c.transform.position, c.GetComponent<Rigidbody>().velocity, scanInterval - scanRecoveryPeriod);
						targetList.Remove(sp.gameObject);
					}
				}
			}
		}
	}

	private IEnumerator UpdateScan(float waitTime)
	{
		while (true)
		{
			UpdateScan();
			yield return new WaitForSeconds(waitTime);
		}
	}
}
