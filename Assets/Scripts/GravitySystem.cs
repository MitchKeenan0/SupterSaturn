using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySystem : MonoBehaviour
{
	public float updateInterval = 0.1f;

	private List<Gravity> gravityList;
	private IEnumerator loadCoroutine;
	private IEnumerator surroundingCheckCoroutine;
	private List<List<Rigidbody>> rbMasterList;

	public List<Rigidbody> GetRBList(int index) { return rbMasterList[index]; }

	void Awake()
	{
		gravityList = new List<Gravity>();
		rbMasterList = new List<List<Rigidbody>>();
		loadCoroutine = LoadWait(0.3f);
		StartCoroutine(loadCoroutine);
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitGravitySystem();
	}

	void InitGravitySystem()
	{
		if (gravityList == null)
			gravityList = new List<Gravity>();
		if (rbMasterList == null)
			rbMasterList = new List<List<Rigidbody>>();

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
		List<Rigidbody> rbList = rbMasterList[gravityIndex];
		Gravity gravity = null;
		if ((gravityIndex < gravityList.Count) && (gravityList[gravityIndex] != null))
		{
			gravity = gravityList[gravityIndex];
			Rigidbody[] nearRbs = FindObjectsOfType<Rigidbody>();
			int numNears = nearRbs.Length;
			for (int i = 0; i < numNears; i++)
			{
				if (nearRbs[i] != null)
				{
					Rigidbody r = nearRbs[i];
					if ((Vector3.Distance(gravity.bodyTransform.position, r.position) < gravity.radius)
						&& (!rbList.Contains(r)))
					{
						rbList.Add(r);
					}
				}
			}
		}

		// check over for objects beyond reach
		if ((rbList != null) && (gravity != null))
		{
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
		}

		gravity.SetRbList(rbList);
	}

	List<Rigidbody> CreateRbList()
	{
		List<Rigidbody> rbList = new List<Rigidbody>();
		return rbList;
	}
}
