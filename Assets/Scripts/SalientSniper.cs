using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalientSniper : Salient
{


	public override void Start()
	{
		base.Start();

	}

	public override void InitSalience()
	{
		base.InitSalience();
	}

	public override void Sense()
	{
		base.Sense();
		if (bPlayerLOS)
			Attac();
	}

	public override void Move(Vector3 moveDirection, ForceMode forceMode)
	{
		base.Move(moveDirection, forceMode);
	}

	public override void Attac()
	{
		base.Attac();
		Vector3 attacForward = (playerObject.transform.position - transform.position).normalized;
		Quaternion attacRotation = Quaternion.LookRotation(attacForward);
		GameObject projectileObj = Instantiate(attacPrefab, transform.position, attacRotation);
		Projectile pr = projectileObj.GetComponent<Projectile>();
		if (pr != null)
			pr.ArmProjectile(null);
	}

	//void Update()
	//{
	//	if (bPlayerLOS)
	//	{
	//		Vector3 moveVector = playerObject.transform.position - transform.position;
	//		moveVector = moveVector.normalized * moveSpeed;
	//		Move(moveVector, ForceMode.Force);
	//	}
	//}
}
