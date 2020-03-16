using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBoost : Ability
{
	public float boostPower = 3f;

	private Spacecraft mySpacecraft;
	private Autopilot autopilot;
	private float originalEnginePower = 0;

    void Start()
    {
		mySpacecraft = GetComponentInParent<Spacecraft>();
		autopilot = GetComponentInParent<Autopilot>();
		originalEnginePower = mySpacecraft.mainEnginePower;
    }

	public override void StartAbility()
	{
		base.StartAbility();
		mySpacecraft.mainEnginePower = boostPower;
		if (mySpacecraft.GetMainEngineVector() == Vector3.zero)
			autopilot.FireEngineBurn(1.6f, false);

		GameObject startEffect = Instantiate(startEffectPrefab, transform.position, transform.rotation);
		Destroy(startEffect, 1f);
	}

	public override void UpdateAbility()
	{
		base.UpdateAbility();

	}

	public override void EndAbility()
	{
		base.EndAbility();
		mySpacecraft.mainEnginePower = originalEnginePower;
	}
}
