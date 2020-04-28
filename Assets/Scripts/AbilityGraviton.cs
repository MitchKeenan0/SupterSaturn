using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGraviton : Ability
{
	public GravitonBeam beamPrefab;
	public float beamSpeed = 1000f;

	private AbilityTargetingHUD abilityTargeting;
	private GravitonBeam beam;
	private Spacecraft spacecraft = null;
	private Transform targetTransform = null;
	private bool bUpdating = false;

	public override void Start()
	{
		base.Start();
		abilityTargeting = FindObjectOfType<AbilityTargetingHUD>();
		spacecraft = GetComponentInParent<Spacecraft>();
	}

	void Update()
	{
		if (bUpdating)
		{
			if (targetTransform == null)
			{
				targetTransform = abilityTargeting.DirectTargeting();
			}
			else
			{
				LaunchGraviton();
				bUpdating = false;
			}
		}
	}

	public override void StartAbility()
	{
		bUpdating = true;

		abilityTargeting.SetActive(true, spacecraft);

		if (startEffectPrefab != null)
		{
			GameObject startEffect = Instantiate(startEffectPrefab, transform.position, transform.rotation);
			Destroy(startEffect, 1f);
		}
	}

	void LaunchGraviton()
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
