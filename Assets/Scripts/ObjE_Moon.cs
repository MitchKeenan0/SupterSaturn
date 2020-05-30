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

	public override void Init(Vector3 offset, Quaternion rotation)
	{
		base.Init(offset, rotation);
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

			if (offset != Vector3.zero)
			{
				PlanetArm planetArm = element.GetComponent<PlanetArm>();
				int numExistingMoons = FindObjectsOfType<ObjE_Moon>().Length;
				float armLength = offset.magnitude * (numExistingMoons + 1) * distanceScale * Random.Range(0.9f, 1.1f);
				planetArm.SetLength(armLength);
			}
		}
	}

	public override void UpdateElement()
	{
		base.UpdateElement();

	}
}
