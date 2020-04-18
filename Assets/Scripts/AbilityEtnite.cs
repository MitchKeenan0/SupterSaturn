using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEtnite : Ability
{
	public EtniteBeam beamPrefab;
	public float beamSpeed = 1000f;
	public float baseOutput = 1f;
	public float chargeRate = 1f;
	public float maxCharge = 5f;

	private EtniteBeam beam = null;
	private AbilityTargetingHUD abilityTargeting = null;
	private Spacecraft spacecraft = null;
	private SkillPanelAbilityPanel myAbilityPanel;
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
		if (GetUIObject() != null)
			myAbilityPanel = GetUIObject().GetComponent<SkillPanelAbilityPanel>();
	}

	void Update()
	{
		if (bCharging)
		{
			charge += Time.deltaTime * chargeRate;
			charge = Mathf.Clamp(charge, 0f, maxCharge);
			UpdateChargeUI(charge);
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
				bCharging = false;
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

	void UpdateChargeUI(float value)
	{
		if (!myAbilityPanel && (GetUIObject() != null))
			myAbilityPanel = GetUIObject().GetComponent<SkillPanelAbilityPanel>();
		if (myAbilityPanel != null)
		{
			float percentOfCharge = charge / maxCharge;
			myAbilityPanel.SetUIChargePercent(percentOfCharge);
		}
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

		charge = 0f;
		UpdateChargeUI(0f);
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

		bUpdating = false;
		bCharging = false;
		targetTransform = null;
		if (beam != null)
			beam.SetEnabled(false);
	}

	public override void CancelAbility()
	{
		base.CancelAbility();
		EndAbility();
		if (myAbilityPanel != null)
			myAbilityPanel.SetUIChargePercent(0f);
	}
}
