using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGraviton : Ability
{
	void Start()
	{
		//
	}

	public override void StartAbility()
	{
		base.StartAbility();
		//

		GameObject startEffect = Instantiate(startEffectPrefab, transform.position, transform.rotation);
		Destroy(startEffect, 1f);
	}

	public override void UpdateAbility()
	{
		base.UpdateAbility();
		//
	}

	public override void EndAbility()
	{
		base.EndAbility();
		//
	}
}
