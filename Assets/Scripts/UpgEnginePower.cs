using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgEnginePower : Upgrade
{
    
    void Start()
    {
        
    }

	public override void EffectUpgrade()
	{
		base.EffectUpgrade();
		Spacecraft sp = FindObjectOfType<TeamFleetHUD>().GetTeamList(0)[0];
		if (sp != null)
		{
			sp.mainEnginePower += value;
			Debug.Log("engine power + " + value);
		}
	}
}
