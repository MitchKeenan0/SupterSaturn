using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentityManager : MonoBehaviour
{
	public GameObject identityPrefab;
	public int identityCount = 4;

	void Awake()
	{
		InitIdentities();
	}

	public void InitIdentities()
	{
		for (int i = 0; i < identityCount; i++)
			CreateIdentity();
	}

	Identity CreateIdentity()
	{
		GameObject idObj = Instantiate(identityPrefab, Vector3.zero, Quaternion.identity);
		Identity id = idObj.GetComponent<Identity>();
		return id;
	}
}
