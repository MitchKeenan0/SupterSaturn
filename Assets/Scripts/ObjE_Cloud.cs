using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjE_Cloud : ObjectiveElement
{
	public int cloudChildren = 10;
	public float cloudSpread = 100f;
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
			element.SetActive(true);
			element.transform.position = Random.insideUnitSphere * (Random.Range(spawnRangeMinimum, spawnRangeMaximum));

			for (int i = 0; i < cloudChildren; i++)
			{
				Vector3 spreadPosition = element.transform.position + (Random.insideUnitSphere * (cloudSpread));
				spreadPosition.y = 0f;
				GameObject duplicate = Instantiate(elementPrefab, spreadPosition, Quaternion.identity);
				duplicate.transform.SetParent(element.transform);
			}
		}
	}

	public override void UpdateElement()
	{
		base.UpdateElement();

	}
}
