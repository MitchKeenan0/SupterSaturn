using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRangeControl : MonoBehaviour
{
	public float rangeIncrement = 3f;

	private InputController inputController;
	private OrbitController orbitController;

	void Start()
    {
		inputController = FindObjectOfType<InputController>();
		orbitController = FindObjectOfType<OrbitController>();
    }

	public void Increase()
	{
		orbitController.ModifyOrbitRange(rangeIncrement);
	}

	public void Decrease()
	{
		orbitController.ModifyOrbitRange(-rangeIncrement);
	}
}
