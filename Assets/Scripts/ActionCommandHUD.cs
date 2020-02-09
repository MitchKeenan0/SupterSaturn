using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCommandHUD : MonoBehaviour
{
	public Transform targetPanel;
	public Transform targetEliminatedPanel;

	private Transform targetTransform;
	private Camera cameraMain;
	private IEnumerator disengageCoroutine;

    void Start()
    {
		cameraMain = Camera.main;
		targetPanel.gameObject.SetActive(false);
		targetEliminatedPanel.gameObject.SetActive(false);
	}

	void Update()
	{
		if ((targetPanel != null) && (targetTransform != null))
		{
			Vector3 targetScreenPosition = cameraMain.WorldToScreenPoint(targetTransform.position);
			targetPanel.position = targetScreenPosition;

			Spacecraft sp = targetTransform.GetComponent<Spacecraft>();
			if ((sp != null && !sp.IsAlive()) || !sp)
			{
				targetEliminatedPanel.gameObject.SetActive(true);
				disengageCoroutine = Disengage(1f);
				StartCoroutine(disengageCoroutine);
			}

			if (targetEliminatedPanel.gameObject.activeInHierarchy)
				targetEliminatedPanel.position = targetPanel.position;
		}
	}

	private IEnumerator Disengage(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		targetPanel.gameObject.SetActive(false);
		targetEliminatedPanel.gameObject.SetActive(false);
	}

	public void SetTarget(Transform target)
	{
		targetTransform = target;
		if (target != null)
			targetPanel.gameObject.SetActive(true);
		else
			targetPanel.gameObject.SetActive(false);
	}
}
