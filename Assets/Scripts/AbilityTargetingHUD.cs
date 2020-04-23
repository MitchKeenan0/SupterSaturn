using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTargetingHUD : MonoBehaviour
{
	public GameObject targetPanelPrefab;
	public GameObject spatialTargetPanel;

	private Spacecraft spacecraft;
	private ObjectManager objectManager;
	private Camera cameraMain;
	private RaycastManager raycastManager;
	private Animator animator;
	private Transform targetTransform = null;
	private List<AbilityTargetPanel> targetPanelList;
	private List<Spacecraft> spacecraftList;
	private bool bActive = false;

    void Start()
    {
		targetPanelList = new List<AbilityTargetPanel>();
		spacecraftList = new List<Spacecraft>();
		objectManager = FindObjectOfType<ObjectManager>();
		animator = GetComponent<Animator>();
		cameraMain = Camera.main;
		raycastManager = FindObjectOfType<RaycastManager>();
		if (spatialTargetPanel != null)
			spatialTargetPanel.SetActive(false);
	}

	GameObject SpawnTargetPanel(Transform tr)
	{
		GameObject panel = Instantiate(targetPanelPrefab, transform);
		AbilityTargetPanel atp = panel.GetComponent<AbilityTargetPanel>();
		atp.LoadTarget(tr);
		targetPanelList.Add(atp);
		return panel;
	}

	public void SetActive(bool value, Spacecraft sp)
	{
		bActive = value;
		spacecraft = sp;
		targetTransform = null;
	}

	public Transform DirectTargeting()
	{
		Transform target = null;
		int objCount = 0;
		if (objectManager.GetSpacecraftList() != null)
			objCount = objectManager.GetSpacecraftList().Count;

		if (objCount > 0)
		{
			List<Spacecraft> objList = new List<Spacecraft>();
			objList = objectManager.GetSpacecraftList();
			if (spacecraftList.Count < objCount)
			{
				for (int i = 1; i < objCount + 1; i++)
				{
					if (i > spacecraftList.Count)
					{
						Spacecraft sp = objList[i - 1];
						if (sp != spacecraft)
						{
							SpawnTargetPanel(sp.transform);
							spacecraftList.Add(objList[i - 1]);
						}
					}
				}
			}
		}

		if (targetTransform != null)
		{
			target = targetTransform;
		}
		else
		{
			int numTargets = targetPanelList.Count;
			for (int i = 0; i < numTargets; i++)
			{
				if ((spacecraftList.Count > i) && (spacecraftList[i] != null))
				{
					AbilityTargetPanel atp = targetPanelList[i];
					Spacecraft sp = spacecraftList[i];
					if (sp.IsAlive())
					{
						Vector3 screenPosition = cameraMain.WorldToScreenPoint(sp.transform.position);
						atp.transform.position = screenPosition;
						atp.SetEnabled(true);
					}
				}
			}
		}

		return target;
	}

	public void SelectTarget(Transform target)
	{
		if (target != null)
		{
			targetTransform = target;
			ClearTargets();
		}
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
					if ((hit.transform != null) && (hit.transform.gameObject != originTransform.gameObject)
						&& (!originTransform.IsChildOf(hit.transform)))
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

	void ClearTargets()
	{
		int numTargets = targetPanelList.Count;
		for (int i = 0; i < numTargets; i++)
		{
			AbilityTargetPanel atp = targetPanelList[i];
			atp.SetEnabled(false);
		}
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
			targetScreenPosition.z = cameraMain.transform.position.z + 10;
			spatialTargetPanel.transform.position = targetScreenPosition;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		spatialTargetPanel.SetActive(false);
	}
}
