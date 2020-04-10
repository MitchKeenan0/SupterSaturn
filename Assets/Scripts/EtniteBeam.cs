using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtniteBeam : Beam
{
	private float chargeEnergy = 0f;

    void Start()
    {
        
    }

	public void SetChargeEnergy(float value)
	{
		chargeEnergy = value;
		float backgroundW = value * 0.1f;
		Color beamChargeColor = new Color(value, backgroundW, backgroundW);
		line.startColor = beamChargeColor;
		line.endColor = beamChargeColor * 0.6f;
	}

	public override void SetEnabled(bool value)
	{
		base.SetEnabled(value);

	}

	public override void SetTarget(Transform value)
	{
		base.SetTarget(value);

	}

	public override void SetOwner(Transform value)
	{
		base.SetOwner(value);

	}

	public override void SetLinePositions(Vector3 start, Vector3 end, float speed)
	{
		base.SetLinePositions(start, end, speed);
		Vector3 lineEndPosition = line.GetPosition(1);
		if (targetTransform != null)
		{
			float beamDistToTarget = Vector3.Distance(lineEndPosition, targetTransform.position);
			if (beamDistToTarget <= 10f)
			{
				Health health = targetTransform.GetComponent<Health>();
				if (health != null)
				{
					int damage = Mathf.FloorToInt(chargeEnergy);
					health.ModifyHealth(-damage, owningTransform);
				}
			}
		}
	}

	public override void UpdateBeam()
	{
		base.UpdateBeam();

	}
}
