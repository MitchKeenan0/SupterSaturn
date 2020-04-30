using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalientHunter : Salient
{


	public override void Start()
    {
		base.Start();

    }

	public override void InitSalience()
	{
		base.InitSalience();
		//Debug.Log("init");
	}

	public override void Sense()
	{
		base.Sense();
		//Debug.Log("sense");
	}

	public override void Move(Vector3 moveDirection, ForceMode forceMode)
	{
		base.Move(moveDirection, forceMode);
		//Debug.Log("moving");
	}

	public override void Attac()
	{
		base.Attac();

	}

	void Update()
	{
		if (bPlayerLOS)
		{
			Vector3 moveVector = playerObject.transform.position - transform.position;
			moveVector = moveVector.normalized * moveSpeed;
			Move(moveVector, ForceMode.Force);
		}
	}
}
