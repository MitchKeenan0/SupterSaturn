using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgPowerplant : Upgrade
{
    void Start()
    {
        
    }

	public override void EffectUpgrade()
	{
		base.EffectUpgrade();
		Powerplant pp = FindObjectOfType<TeamFleetHUD>().GetTeamList(0)[0].GetComponentInChildren<Powerplant>();
		if (pp != null)
		{
			pp.RaiseMaxPower(value);
		}
	}
}
