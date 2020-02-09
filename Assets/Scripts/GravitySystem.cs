﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySystem : MonoBehaviour
{
	public float updateInterval = 0.1f;

	private List<Gravity> gravityList;
	private IEnumerator surroundingCheckCoroutine;
	private List<List<Rigidbody>> rbMasterList;

	public List<Rigidbody> GetRBList(int index) { return rbMasterList[index]; }

	void Awake()
	{
		gravityList = new List<Gravity>();
		rbMasterList = new List<List<Rigidbody>>();
	}

	public void InitGravitySystem()
	{
		Gravity[] allGravities = FindObjectsOfType<Gravity>();
		foreach (Gravity g in allGravities)
		{
			gravityList.Add(g);
			List<Rigidbody> grbList = CreateRbList();
			rbMasterList.Add(grbList);
		}

		surroundingCheckCoroutine = SurroundingCheck(updateInterval);
		StartCoroutine(surroundingCheckCoroutine);
	}

	private IEnumerator SurroundingCheck(float intervalTime)
	{
		while (true)
		{
			yield return new WaitForSeconds(intervalTime);

			int numGravity = gravityList.Count;
			if (numGravity > 0)
			{
				for (int i = 0; i < numGravity; i++)
					UpdateGravities(i);
			}
		}
	}

	void UpdateGravities(int gravityIndex)
	{
		Gravity gravity = gravityList[gravityIndex];
		List<Rigidbody> rbList = rbMasterList[gravityIndex];

		Collider[] nears = Physics.OverlapSphere(gravity.bodyTransform.position, gravity.radius);
		int numNears = nears.Length;
		for (int i = 0; i < numNears; i++)
		{
			if (nears[i] != null)
			{
				Rigidbody r = nears[i].gameObject.GetComponent<Rigidbody>();
				if (r != null)
				{
					if (!rbList.Contains(r))
						rbList.Add(r);
				}
			}
		}

		// check over for objects beyond reach
		int numRbs = rbList.Count;
		if (numRbs > 0)
		{
			Vector3 myPos = gravity.bodyTransform.position;
			for (int i = 0; i < numRbs; i++)
			{
				if ((i < rbList.Count) && (rbList[i] != null))
				{
					float distance = Vector3.Distance(rbList[i].transform.position, myPos);
					if (distance >= gravity.radius)
						rbList.RemoveAt(i);
				}
			}
		}

		gravity.SetRbList(rbList);
	}

	List<Rigidbody> CreateRbList()
	{
		List<Rigidbody> rbList = new List<Rigidbody>();
		return rbList;
	}
}
