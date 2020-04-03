using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTargetingHUD : MonoBehaviour
{
	public GameObject targetPanelPrefab;

	private Camera cameraMain;
	private RaycastManager raycastManager;
	private List<AbilityTargetPanel> targetPanelList;
	private bool bActive = false;

    void Start()
    {
		targetPanelList = new List<AbilityTargetPanel>();
		cameraMain = Camera.main;
		raycastManager = FindObjectOfType<RaycastManager>();
	}

    void Update()
    {
		if (bActive)
		{
			UpdateTargets();
		}
    }

	void UpdateTargets()
	{
		int numTargets = targetPanelList.Count;
		for(int i = 0; i < numTargets; i++)
		{

		}
	}

	GameObject SpawnTargetPanel()
	{
		GameObject panel = Instantiate(targetPanelPrefab, transform);
		AbilityTargetPanel atp = panel.GetComponent<AbilityTargetPanel>();
		targetPanelList.Add(atp);
		return panel;
	}

	public Vector3 SpatialTargeting(Transform originTransform)
	{
		Vector3 targetVector = Vector3.zero;
		if (Input.touchCount > 0)
		{
			Touch tap = Input.GetTouch(0);
			Ray ray = cameraMain.ScreenPointToRay(tap.position);
			RaycastHit[] hits = Physics.RaycastAll(ray);
			if (hits.Length > 0)
			{
				for(int i = 0; i < hits.Length; i++)
				{
					RaycastHit hit = hits[i];
					if ((hit.transform != null) && (hit.transform.gameObject != originTransform.gameObject))
					{
						targetVector = hit.point;
						break;
					}
				}
			}
		}
		
		return targetVector;
	}
}
