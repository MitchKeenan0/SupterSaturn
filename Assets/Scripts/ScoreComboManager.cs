using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreComboManager : MonoBehaviour
{
	public float intervalForCombo = 0.5f;
	public Color[] comboTierColors;

	private float timeAtLastScore = 0f;
	private int currentComboValue = 0;

    void Start()
    {
        
    }

	public int Score(int value)
	{
		int comboScore = value;
		float deltaTime = Time.time - timeAtLastScore;
		if (deltaTime <= intervalForCombo)
		{
			currentComboValue += value;
		}
		else
		{
			currentComboValue = value;
		}
		comboScore = currentComboValue;
		timeAtLastScore = Time.time;
		return comboScore;
	}

	public Color GetTierColor()
	{
		Color tierColor = Color.white;
		if (currentComboValue < comboTierColors.Length)
			tierColor = comboTierColors[currentComboValue];
		return tierColor;
	}
}
