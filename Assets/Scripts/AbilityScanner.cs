using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScanner : Ability
{
	public Scan scanObjectPrefab;

	private ContextHeader contextHeader;
	private AbilityTargetingHUD abilityTargeting;
	private Scan scan;
	private ScoreHUD scoreHud;
	private Vector3 targetVector = Vector3.zero;
	private bool bUpdating = false;
	private bool bScannerHit = false;
	private int hits = 0;

	void Start()
    {
		contextHeader = FindObjectOfType<ContextHeader>();
		abilityTargeting = FindObjectOfType<AbilityTargetingHUD>();
		scoreHud = FindObjectOfType<ScoreHUD>();
    }

	public override void StartAbility()
	{
		targetVector = Vector3.zero;
		bScannerHit = false;
		bUpdating = true;
		hits = 0;
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

	public void Hit(int value)
	{
		if (!bScannerHit)
		{
			bScannerHit = true;
			hits = value;
		}
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
			scan.transform.position = transform.position;
			Vector3 toTarget = (targetVector - transform.position);
			Quaternion scanRotation = Quaternion.LookRotation(toTarget, Vector3.up);
			scan.InitScan(this, scanRotation, duration);
			scan.SetEnabled(true);
		}
	}

	public override void UpdateAbility()
	{
		base.UpdateAbility();
		
	}

	public override void EndAbility()
	{
		base.EndAbility();
		scan.transform.position = transform.position;
		scan.SetEnabled(false);
		targetVector = Vector3.zero;
		if (scoreHud != null)
		{
			if (!bScannerHit)
				scoreHud.ToastContext("-- Out of Range --");
			else
			{
				if (hits == 0)
					scoreHud.ToastContext("-- No Hit --");
				else
					scoreHud.ToastContext("Scan Hit +" + Factorial(hits));
			}
		}
	}

	int Factorial(int value)
	{
		int final = 0;
		for(int i = 0; i < value; i++)
			final += (i + 1);
		return final;
	}

	public override void CancelAbility()
	{
		base.CancelAbility();
		bUpdating = false;
		if (contextHeader != null)
			contextHeader.SetContextHeader("");
	}
}
