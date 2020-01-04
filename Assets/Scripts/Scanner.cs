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
	private Spacecraft spacecraft;
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
		spacecraft = GetComponentInParent<Spacecraft>();
		predictionHud = FindObjectOfType<TargetPredictionHUD>();

		meshRenderer = GetComponentInChildren<MeshRenderer>();
		scanRender = meshRenderer.gameObject;
		originalAlpha = meshRenderer.material.color.a;
		meshRenderer.enabled = false;

		scanCoroutine = LoopingScanLaunch(scanInterval + scanRecoveryPeriod + Random.Range(-intervalDeviation, intervalDeviation));
		StartCoroutine(scanCoroutine);
	}

	void LaunchScan()
	{
		targetList.Clear();
		currentRadius = 0f;
		timeAtScan = Time.time;
		updateCoroutine = UpdateScan(updateInterval);
		StartCoroutine(updateCoroutine);
	}

	void UpdateScan()
	{
		if (spacecraft.GetAgent().teamID == 0)
		{
			UpdateScanRender();
			meshRenderer.enabled = true;
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
					if ((sp != null) && (sp != spacecraft))
					{
						if (sp.GetAgent().teamID != spacecraft.GetAgent().teamID)
						{
							if (spacecraft.GetAgent().LineOfSight(sp.transform.position, sp.transform))
							{
								if (Vector3.Distance(sp.transform.position, transform.position) <= maxRadius)
								{
									targetList.Add(hit.gameObject);
									sp.GetComponent<Spacecraft>().AddMarkValue(1);

									if (spacecraft.GetAgent().teamID == 0)
										predictionHud.SetPrediction(sp, Vector3.zero, Vector3.zero, 0f);
								}
							}
						}
					}
				}
			}
		}

		currentRadius = Mathf.Lerp(currentRadius, maxRadius, (scanSpeed * Time.deltaTime));
		bool bScanFinished = (Time.time - timeAtScan) >= scanInterval;
		if (bScanFinished)
		{
			ClearTargets();
			StopCoroutine(updateCoroutine);
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
		foreach (GameObject c in targetList)
		{
			if (c != null)
			{
				Spacecraft sp = c.GetComponent<Spacecraft>();
				sp.AddMarkValue(-1);
				if (spacecraft.GetAgent().teamID == 0)
				{
					predictionHud.SetPrediction(sp, c.transform.position, c.GetComponent<Rigidbody>().velocity, scanInterval);
				}
			}
		}
		targetList.Clear();
		spacecraft.GetAgent().SuggestTarget(null);
	}

	private IEnumerator LoopingScanLaunch(float waitTime)
	{
		while (true)
		{
			LaunchScan();
			yield return new WaitForSeconds(waitTime);
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
