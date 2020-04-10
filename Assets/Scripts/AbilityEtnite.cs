using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEtnite : Ability
{
	public EtniteBeam beamPrefab;
	public float beamSpeed = 1000f;
	public float baseOutput = 1f;
	public float chargeRate = 1f;
	public float maxCharge = 9999f;

	private EtniteBeam beam = null;
	private AbilityTargetingHUD abilityTargeting = null;
	private Spacecraft spacecraft = null;
	private Transform targetTransform = null;
	private Vector3 trackTargetVector = Vector3.zero;
	private bool bUpdating = false;
	private bool bCharging = false;
	private float charge = 0f;

	void Start()
    {
		spacecraft = GetComponentInParent<Spacecraft>();
		abilityTargeting = FindObjectOfType<AbilityTargetingHUD>();
		trackTargetVector = transform.forward;
    }

	void Update()
	{
		if (bCharging)
		{
			charge += Time.deltaTime * chargeRate;
		}

		if (bUpdating)
		{
			if (targetTransform == null)
			{
				targetTransform = abilityTargeting.DirectTargeting();
			}
			else
			{
				LaunchEtnite();
				bUpdating = false;
			}
		}
	}

	public override void StartAbility()
	{
		bUpdating = true;
		bCharging = true;

		abilityTargeting.SetActive(true, spacecraft);

		if (startEffectPrefab != null)
		{
			GameObject startEffect = Instantiate(startEffectPrefab, transform.position, transform.rotation);
			Destroy(startEffect, 1f);
		}
	}

	void LaunchEtnite()
	{
		base.StartAbility();
	}

	public override void FireAbility()
	{
		base.FireAbility();

		abilityTargeting.SetActive(false, spacecraft);

		if (!beam)
		{
			beam = Instantiate(beamPrefab, transform);
			beam.SetOwner(transform);
		}

		if ((beam != null) && (targetTransform != null))
		{
			beam.SetChargeEnergy(charge);
			beam.SetEnabled(true);
			beam.SetTarget(targetTransform);
			beam.SetLinePositions(transform.position, transform.position, 0f);
		}
	}

	public override void UpdateAbility()
	{
		base.UpdateAbility();

		if ((beam != null) && (targetTransform != null))
		{
			beam.SetLinePositions(transform.position, targetTransform.position, beamSpeed);
		}
	}

	public override void EndAbility()
	{
		base.EndAbility();

		targetTransform = null;
		if (beam != null)
			beam.SetEnabled(false);
	}
}
