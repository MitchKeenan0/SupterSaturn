using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerplant : MonoBehaviour
{
	public float maxPower = 10f;
	public float startPowerFactor = 0.5f;
	public float rechargeRate = 1f;

	public float GetPower() { return currentPower; }
	private float currentPower = 0f;

	private float powerDraw = 0f;

    void Start()
    {
		currentPower = maxPower * startPowerFactor;
    }

    void Update()
    {
        if (currentPower < maxPower)
		{
			currentPower = Mathf.MoveTowards(currentPower, maxPower, Time.deltaTime * rechargeRate);
		}

		currentPower += (powerDraw * Time.deltaTime);
		currentPower = Mathf.Clamp(currentPower, 0f, maxPower);

		if (currentPower == 0f)
		{
			// todo burnout
		}
    }

	public void InstantCost(float value)
	{
		currentPower -= value;
	}

	public void AddPowerDrawOffset(float value)
	{
		powerDraw += value;
		powerDraw = Mathf.Clamp(powerDraw, -maxPower, 0f);
	}
}
