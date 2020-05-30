using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjE_Hostile : ObjectiveElement
{
	public float spawnRangeMinimum = 300f;
	public float spawnRangeMaximum = 1000f;

	private GameObject element = null;

	void Start()
	{
		
	}

	public override void Init(Vector3 offset, Quaternion rotation)
	{
		base.Init(offset, rotation);
		element = GetElement();
		if (element != null)
		{
			float spawnRange = Random.Range(spawnRangeMinimum, spawnRangeMaximum);
			Vector3 spawnPosition = Random.onUnitSphere * spawnRange;
			spawnPosition.y = 0f;
			element.transform.position = spawnPosition;
			element.SetActive(true);
		}
	}

	public override void UpdateElement()
	{
		base.UpdateElement();

	}
}
