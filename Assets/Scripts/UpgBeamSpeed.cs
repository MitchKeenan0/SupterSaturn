using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgBeamSpeed : Upgrade
{
    
    void Start()
    {
        
    }

	public override void EffectUpgrade()
	{
		base.EffectUpgrade();
		ScanRay scanRay = FindObjectOfType<TeamFleetHUD>().GetTeamList(0)[0].GetComponentInChildren<ScanRay>();
		if (scanRay != null)
		{
			scanRay.raySpeed += value;
			Debug.Log("ray speed + " + value);
		}
		else
		{
			Debug.Log("no scan ray");
		}
	}
}
