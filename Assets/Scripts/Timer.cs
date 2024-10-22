﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	public Text valueText;
	public Text decimalText;
	public int integerDigits = 2;
	public bool bDecimal = false;
	public int decimalDigits = 2;

	private bool bUpdating = true;

    void Start()
    {
        
    }
    
    void Update()
    {
		if (bUpdating)
		{
			float currentTime = Time.timeSinceLevelLoad;
			int intTime = Mathf.FloorToInt(currentTime);
			string stringTime = currentTime.ToString();

			/// big second counter
			string valueString = intTime.ToString();
			valueText.text = valueString;

			/// small decimal counter
			float decimalTime = Mathf.Clamp((currentTime - intTime) * 100f, 0f, 99f);
			string decimalString = decimalTime.ToString("F0");
			//decimalString = 
			if (decimalTime < 10)
				decimalString = "0" + decimalString;
			decimalText.text = decimalString;
		}
    }

	public void Freeze()
	{
		bUpdating = false;
	}
}
