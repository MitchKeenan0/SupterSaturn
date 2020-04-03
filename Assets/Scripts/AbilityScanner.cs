using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScanner : Ability
{
	public Scan scanObjectPrefab;

	private ContextHeader contextHeader;
	private AbilityTargetingHUD abilityTargeting;
	private bool bUpdating = false;
	private Vector3 targetVector = Vector3.zero;
	private Scan scan;

    void Start()
    {
		contextHeader = FindObjectOfType<ContextHeader>();
		abilityTargeting = FindObjectOfType<AbilityTargetingHUD>();
    }

	public override void StartAbility()
	{
		targetVector = Vector3.zero;
		bUpdating = true;
		if (!contextHeader)
			contextHeader = FindObjectOfType<ContextHeader>();
		if (contextHeader != null)
			contextHeader.SetContextHeader("Tap the screen to launch scan");
	}

	void Update()
	{
		if (bUpdating)
		{
			if (targetVector == Vector3.zero)
			{
				targetVector = abilityTargeting.SpatialTargeting(transform);
			}
			else
			{
				bUpdating = false;
				LaunchScan();
			}
		}
	}

	void LaunchScan()
	{
		base.StartAbility();

	}

	public override void FireAbility()
	{
		base.FireAbility();

		if (contextHeader != null)
			contextHeader.SetContextHeader("");

		if (!scan)
		{
			if (scanObjectPrefab != null)
				scan = Instantiate(scanObjectPrefab, null);
		}

		if (scan != null)
		{
			scan.SetOwningScanner(transform);
			scan.transform.position = transform.position;
			Vector3 toTarget = (targetVector - transform.position);
			scan.transform.rotation = Quaternion.LookRotation(toTarget, Vector3.up);
			scan.SetEnabled(true);
		}
	}

	public override void UpdateAbility()
	{
		base.UpdateAbility();

		if (scan != null)
		{
			scan.transform.Translate(Vector3.forward * scan.scanSpeed * Time.deltaTime);
		}
	}

	public override void EndAbility()
	{
		base.EndAbility();
		scan.transform.position = transform.position;
		scan.SetEnabled(false);
	}
}
