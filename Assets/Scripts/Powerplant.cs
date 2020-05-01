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
	private float _maxPower = 0f;

	public float GetMaxPower() { return _maxPower; }
	public void RaiseMaxPower(float value)
	{
		Debug.Log("_maxPower before " + _maxPower);
		_maxPower += value;
		Debug.Log("_maxPower after " + _maxPower);
	}

    void Awake()
    {
		_maxPower = maxPower;
		Debug.Log("_maxPower init " + _maxPower);
		currentPower = _maxPower * startPowerFactor;
    }

    void Update()
    {
        if (currentPower < _maxPower)
		{
			currentPower = Mathf.MoveTowards(currentPower, _maxPower, Time.deltaTime * rechargeRate);
		}

		currentPower += (powerDraw * Time.deltaTime);
		currentPower = Mathf.Clamp(currentPower, 0f, _maxPower);

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
		powerDraw = Mathf.Clamp(powerDraw, -_maxPower, 0f);
	}
}
