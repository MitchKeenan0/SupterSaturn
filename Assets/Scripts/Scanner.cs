using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
	public bool bHitsPlanets = false;
	public float scanInterval = 1f;
	public float intervalDeviation = 0.1f;
	public float scanSpeed = 1f;
	public float maxRadius = 1f;
	public float scanRecoveryPeriod = 0.5f;
	public float maxPointSize = 1f;

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
	///private float pointSize = 0f;
	private bool bScanning = false;
	private bool bUpdating = false;

	public GameObject[] GetTargets() { return targetList.ToArray(); }
	public bool IsScanning() { return bScanning; }

	void Start()
    {
		targetList = new List<GameObject>();
		mySpacecraft = GetComponentInParent<Spacecraft>();
		predictionHud = FindObjectOfType<TargetPredictionHUD>();
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		scanRender = meshRenderer.gameObject;
		originalAlpha = meshRenderer.material.GetFloat("_Opacity");
		meshRenderer.enabled = false;
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
		///pointSize = 0f;
		if (scanRender != null)
			scanRender.transform.localScale = Vector3.one * 0.1f;
		if (meshRenderer != null)
			meshRenderer.material.SetFloat("_Opacity", originalAlpha);
		bUpdating = true;
	}

	void Update()
	{
		if (bUpdating && (Time.timeSinceLevelLoad > 1f))
			UpdateScan();
	}

	void UpdateScan()
	{
		if (((mySpacecraft != null && mySpacecraft.GetAgent() != null))
			|| (mySpacecraft == null))
		{
			UpdateScanRender();
			if ((meshRenderer != null) && !meshRenderer.enabled)
				meshRenderer.enabled = true;
		}

		currentRadius += (scanSpeed * Time.deltaTime);
		bool bScanFinished = (Time.time - timeAtScan) >= scanInterval;
		if (bScanFinished)
		{
			ClearTargets();
			bUpdating = false;
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
					if ((hit.gameObject.activeInHierarchy) && hit.gameObject.GetComponent<Spacecraft>())
					{
						Spacecraft sp = hit.gameObject.GetComponent<Spacecraft>();
						if ((sp != null)
							&& (sp.IsAlive())
							&& (sp.GetAgent().teamID != mySpacecraft.GetAgent().teamID)
							&& mySpacecraft.GetAgent().LineOfSight(sp.transform.position, sp.transform)
							&& (Vector3.Distance(sp.transform.position, transform.position) <= maxRadius))
						{
							targetList.Add(hit.gameObject);
							sp.GetComponent<Spacecraft>().AddMarkValue(1);
						}
					}

					if (bHitsPlanets)
					{
						Planet pt = hit.gameObject.GetComponentInChildren<Planet>();
						if ((pt != null) && (mySpacecraft != null) && (pt.bScannable))
						{
							Color32 scanColor = new Color32(200, 200, 200, 255);
							//pt.GetScanned(mySpacecraft.transform.position, currentRadius, scanColor);
						}
					}
				}
			}
		}
	}

	void UpdateScanRender()
	{
		float safeRadius = Mathf.Clamp(currentRadius, 0.01f, maxRadius);
		if (scanRender != null)
		{
			scanRender.transform.localScale = Vector3.one * safeRadius * 2f;
			scanRender.transform.rotation = Quaternion.identity;
		}

		float percentOfLifetimeRemaining = 1f - (Time.time - timeAtScan) / scanInterval;
		float alpha = Mathf.Clamp(originalAlpha * percentOfLifetimeRemaining, 0f, 1f);
		if (meshRenderer != null)
			meshRenderer.material.SetFloat("_Opacity", alpha);
	}

	void ClearTargets()
	{
		if (targetList == null)
			targetList = new List<GameObject>();
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

	public void BeginScanning()
	{
		bScanning = true;
		scanCoroutine = LoopingScanLaunch(scanInterval + scanRecoveryPeriod + Random.Range(-intervalDeviation, intervalDeviation));
		StartCoroutine(scanCoroutine);
	}

	public void StopScanning()
	{
		bScanning = false;
		StopAllCoroutines();
	}
}
