using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjE_Moon : ObjectiveElement
{
	public float sizeMinimum = 1f;
	public float sizeMaximum = 10f;
	public float distanceScale = 100f;

	private GameObject element;

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

			Planet moonPlanet = element.GetComponentInChildren<Planet>();
			if (moonPlanet != null)
			{
				float moonSize = Random.Range(sizeMinimum, sizeMaximum);
				moonPlanet.SetScale(moonSize);
			}

			PlanetArm planetArm = element.GetComponent<PlanetArm>();
			int numExistingMoons = FindObjectsOfType<ObjE_Moon>().Length;
			planetArm.SetLength((numExistingMoons + 1) * distanceScale * Random.Range(0.9f, 1.1f));

			element.transform.localRotation = Quaternion.Euler(Random.Range(-30f, 30f), Random.Range(-30f, 30f), Random.Range(-30f, 30f));
		}
	}

	public override void UpdateElement()
	{
		base.UpdateElement();

	}
}
