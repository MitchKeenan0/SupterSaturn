using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetFinder : MonoBehaviour
{
	public float startDelayTime = 1f;

	private TouchOrbit touchOrbit = null;
	private TeamFleetHUD teamHud = null;
	private Transform targetTransform = null;
	private bool bActive = false;

    void Start()
    {
		touchOrbit = FindObjectOfType<TouchOrbit>();
		teamHud = FindObjectOfType<TeamFleetHUD>();
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
					touchOrbit.SetAnchorTransform(targetTransform);
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
		//else
		//	touchOrbit.ResetAnchor(true);
	}

	public void SetTarget(Transform value)
	{
		targetTransform = value;
	}
}
