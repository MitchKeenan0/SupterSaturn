using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitonBeam : Beam
{
	void Start()
	{

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
				Rigidbody targetRb = targetTransform.gameObject.GetComponent<Rigidbody>();
				if (targetRb != null)
				{
					Vector3 kineticNeutralizeVector = -10 * targetRb.velocity;
					targetRb.AddForce(kineticNeutralizeVector);
				}
			}
		}
	}

	public override void UpdateBeam()
	{
		base.UpdateBeam();

	}
}
