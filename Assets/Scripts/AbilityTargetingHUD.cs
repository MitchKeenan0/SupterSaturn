using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTargetingHUD : MonoBehaviour
{
	public GameObject targetPanelPrefab;
	public GameObject spatialTargetPanel;

	private Camera cameraMain;
	private RaycastManager raycastManager;
	private Animator animator;
	private List<AbilityTargetPanel> targetPanelList;
	private bool bActive = false;

    void Start()
    {
		targetPanelList = new List<AbilityTargetPanel>();
		animator = GetComponent<Animator>();
		cameraMain = Camera.main;
		raycastManager = FindObjectOfType<RaycastManager>();
		if (spatialTargetPanel != null)
			spatialTargetPanel.SetActive(false);
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
						if (spatialTargetPanel != null)
						{
							spatialTargetCoroutine = SpatialTargetAnim(targetVector, 0.5f);
							StartCoroutine(spatialTargetCoroutine);
						}
						break;
					}
				}
			}
		}
		
		return targetVector;
	}

	private IEnumerator spatialTargetCoroutine;
	private IEnumerator SpatialTargetAnim(Vector3 targetVector, float duration)
	{
		spatialTargetPanel.SetActive(true);
		float timeElapsed = 0f;

		animator.Play("SpatialTarget", 0, 0f);

		while (timeElapsed < duration)
		{
			timeElapsed += Time.deltaTime;
			Vector3 targetScreenPosition = cameraMain.WorldToScreenPoint(targetVector);
			spatialTargetPanel.transform.position = targetScreenPosition;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		spatialTargetPanel.SetActive(false);
	}
}
