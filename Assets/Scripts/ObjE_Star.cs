using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjE_Star : ObjectiveElement
{
	public float starMinSize = 50f;
	public float starMaxSize = 500f;

	private GameObject element = null;
	private Planet planet = null;

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

			planet = element.GetComponent<Planet>();
			if (!planet)
				planet = element.GetComponentInChildren<Planet>();
			if (planet != null)
			{
				planet.gameObject.tag = "Planet";
				float planetSize = Random.Range(starMinSize, starMaxSize);
				planet.SetScale(planetSize);
			}
		}
	}

	public override void UpdateElement()
	{
		base.UpdateElement();

	}
}
