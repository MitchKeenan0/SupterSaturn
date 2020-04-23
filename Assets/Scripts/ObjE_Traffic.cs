using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjE_Traffic : ObjectiveElement
{
	public float spawnRangeMinimum = 300f;
	public float spawnRangeMaximum = 1000f;

	private GameObject element = null;

	void Start()
	{
		
	}

	public override void Init()
	{
		base.Init();
		element = GetElement();
		if (element != null)
		{
			float spawnRange = Random.Range(spawnRangeMinimum, spawnRangeMaximum);
			element.transform.position = Random.onUnitSphere * spawnRange;
			element.SetActive(true);
		}
	}

	public override void UpdateElement()
	{
		base.UpdateElement();

	}
}
