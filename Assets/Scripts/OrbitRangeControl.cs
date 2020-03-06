using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRangeControl : MonoBehaviour
{
	public float rangeIncrement = 3f;

	private InputController inputController;

    void Start()
    {
		inputController = FindObjectOfType<InputController>();
    }

	public void Increase()
	{
		inputController.IncreaseOrbit(rangeIncrement);
	}

	public void Decrease()
	{
		inputController.DecreaseOrbit(rangeIncrement);
	}
}
