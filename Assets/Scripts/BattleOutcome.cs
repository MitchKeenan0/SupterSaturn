﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleOutcome : MonoBehaviour
{
	public GameObject conclusionPanel;
	public GameObject scorePanel;
	public GameObject optionPanel;
	public Text conclusionText;
	public Text scoreText;
	public Text lostText;
	public Text totalText;
	public Text playerNameText;
	public float slowMoUpdateInterval = 0.02f;

	private Game game;
	private Player player;
	private int playerScore = 0;
	private int playerLost = 0;

    void Awake()
    {
		game = FindObjectOfType<Game>();
		player = FindObjectOfType<Player>();

		conclusionPanel.SetActive(false);
		scorePanel.SetActive(false);
		optionPanel.SetActive(false);
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

		scoreText.text = "+" + playerScore.ToString();
		lostText.text = playerLost.ToString();
		playerNameText.text = player.playerName;
		conclusionPanel.SetActive(true);
		scorePanel.SetActive(true);
		optionPanel.SetActive(true);

		if (game != null)
		{
			int totalNewChevrons = playerScore + playerLost;
			if (playerScore >= Mathf.Abs(playerLost))
				totalNewChevrons *= (playerScore - Mathf.Abs(playerLost));

			game.UpdateChevronAccount(totalNewChevrons);
			string conference = "+";
			if (totalNewChevrons < 0)
				conference = "";
			totalText.text = conference + totalNewChevrons.ToString() + "    " + "<size=50>" + game.GetChevrons().ToString() + "</size>";
		}
		
		game.SaveGame();
	}

	public void Reset()
	{
		playerScore = playerLost = 0;
		conclusionPanel.SetActive(false);
		scorePanel.SetActive(false);
	}

	public void Continue()
	{
		SceneManager.LoadScene("CampaignScene");
	}

	public void Return()
	{
		SceneManager.LoadScene("FleetScene");
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
