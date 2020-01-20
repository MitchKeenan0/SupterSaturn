﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleOutcome : MonoBehaviour
{
	public GameObject conclusionPanel;
	public GameObject scorePanel;
	public Text conclusionText;
	public Text scoreText;
	public Text lostText;
	public Text totalText;
	public float slowMoUpdateInterval = 0.02f;

	private Player player;
	private int playerScore = 0;
	private int playerLost = 0;
	private float targetTimescale = 1;

	private IEnumerator slowMoCoroutine;

    void Start()
    {
		player = FindObjectOfType<Player>();
		conclusionPanel.SetActive(false);
		scorePanel.SetActive(false);
    }

	private IEnumerator UpdateTimescale(float updateInterval)
	{
		while (true)
		{
			yield return new WaitForSeconds(updateInterval);
			float current = Time.timeScale;
			float lerped = Mathf.Lerp(current, targetTimescale, updateInterval);
			Time.timeScale = lerped;
		}
	}

	public void AddScore(int value)
	{
		int sureValue = Mathf.Abs(value);
		playerScore += sureValue;
	}

	public void AddLost(int value)
	{
		int sureLoss = Mathf.Abs(value);
		playerLost -= sureLoss;
	}

	public void BattleOver(bool bPlayerWin)
	{
		if (bPlayerWin)
			conclusionText.text = "SUCCESS";
		else
			conclusionText.text = "DEFEAT";
		scoreText.text = playerScore.ToString();
		lostText.text = playerLost.ToString();
		conclusionPanel.SetActive(true);
		scorePanel.SetActive(true);

		if (player != null)
		{
			int totalNewChevrons = playerScore - playerLost;
			player.AddChevrons(totalNewChevrons);
			totalText.text = player.GetChevrons().ToString();
		}
		else
		{
			totalText.text = "!nope";
		}

		targetTimescale = 0.01f;
		slowMoCoroutine = UpdateTimescale(slowMoUpdateInterval);
		StartCoroutine(slowMoCoroutine);
	}

	public void Reset()
	{
		playerScore = playerLost = 0;
		conclusionPanel.SetActive(false);
		scorePanel.SetActive(false);
		Time.timeScale = 1;
		StopAllCoroutines();
	}
}