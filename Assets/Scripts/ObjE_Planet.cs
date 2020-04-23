using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjE_Planet : ObjectiveElement
{
	public float planetMinSize = 50f;
	public float planetMaxSize = 500f;

	private GameObject element = null;
	private Planet planet = null;
	private PlanetArm planetArm = null;

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
			planetArm = element.GetComponent<PlanetArm>();
			if (!planetArm)
				planetArm = element.GetComponentInChildren<PlanetArm>();
			if (planetArm != null)
			{
				planetArm.SetLength(0f);
			}

			planet = element.GetComponent<Planet>();
			if (!planet)
				planet = element.GetComponentInChildren<Planet>();
			if (planet != null)
			{
				planet.gameObject.tag = "Planet";
				float planetSize = Random.Range(planetMinSize, planetMaxSize);
				planet.SetScale(planetSize);
			}
		}
	}

	public override void UpdateElement()
	{
		base.UpdateElement();

	}
}
