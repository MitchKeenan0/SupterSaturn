using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBoost : Ability
{
	public float boostPower = 3f;

	private Spacecraft mySpacecraft;
	private Autopilot autopilot;
	private OrbitController orbitController;
	private float originalEnginePower = 0;

    public override void Start()
    {
		base.Start();
		mySpacecraft = GetComponentInParent<Spacecraft>();
		autopilot = GetComponentInParent<Autopilot>();
		orbitController = FindObjectOfType<OrbitController>();
		originalEnginePower = mySpacecraft.mainEnginePower;
    }

	public override void StartAbility()
	{
		base.StartAbility();
		mySpacecraft.mainEnginePower = boostPower;
		if (mySpacecraft.GetMainEngineVector() == Vector3.zero)
			autopilot.FireEngineBurn(duration, false);
		powerplant.InstantCost(powerCost);

		if (startEffectPrefab != null)
		{
			GameObject startEffect = Instantiate(startEffectPrefab, transform.position, transform.rotation);
			Destroy(startEffect, 1f);
		}
	}

	public override void UpdateAbility()
	{
		base.UpdateAbility();

	}

	public override void EndAbility()
	{
		base.EndAbility();
		autopilot.MainEngineShutdown();
		mySpacecraft.mainEnginePower = originalEnginePower;
	}
}
