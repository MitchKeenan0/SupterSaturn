using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgHealth : Upgrade
{

	void Start()
	{

	}

	public override void EffectUpgrade()
	{
		base.EffectUpgrade();
		Health hp = FindObjectOfType<TeamFleetHUD>().GetTeamList(0)[0].GetComponent<Health>();
		if (hp != null)
		{
			int valueInt = Mathf.RoundToInt(value);
			hp.RaiseMaxHealth(valueInt);
			hp.ModifyHealth(valueInt, null);
		}
	}
}
