using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetFinder : MonoBehaviour
{
	public float startDelayTime = 1f;

	private TeamFleetHUD teamHud = null;
	private Transform targetTransform = null;
	private bool bActive = false;
	private IEnumerator startDelay;

    void Start()
    {
		teamHud = FindObjectOfType<TeamFleetHUD>();
		startDelay = StartDelay(startDelayTime);
		StartCoroutine(startDelay);
    }

	IEnumerator StartDelay(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		bActive = true;
		TargetFind();
	}

	void TargetFind()
	{
		List<Spacecraft> spList = new List<Spacecraft>();
		spList = teamHud.GetTeamList(0);
		if (spList.Count > 0)
		{
			foreach(Spacecraft sp in spList)
			{
				if (sp != null)
				{
					targetTransform = sp.transform;
					break;
				}
			}
		}
	}

	public Transform GetFirstTarget()
	{
		TargetFind();
		return targetTransform;
	}

	public void SetActive(bool value)
	{
		bActive = value;
		if (bActive)
			TargetFind();
	}
}
