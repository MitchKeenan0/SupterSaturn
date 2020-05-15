using System.Collections;
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
	private ScoreHUD scoreHud;
	private int playerScore = 0;
	private int playerLost = 0;

    void Awake()
    {
		game = FindObjectOfType<Game>();
		player = FindObjectOfType<Player>();
		scoreHud = FindObjectOfType<ScoreHUD>();

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
		FindObjectOfType<Timer>().Freeze();

		if (bPlayerWin)
			conclusionText.text = "SUCCESS";
		else
			conclusionText.text = "DEFEAT";

		//if (!scoreHud)
		//	scoreHud = FindObjectOfType<ScoreHUD>();
		//if (scoreHud != null)
		//	playerScore += scoreHud.GetScore();

		ObjectiveType ot = FindObjectOfType<ObjectiveType>();
		if (ot != null)
		{
			playerScore += ot.objectiveValue;
		}

		scoreText.text = "+" + playerScore.ToString();
		lostText.text = playerLost.ToString();
		playerNameText.text = player.playerName;

		conclusionPanel.SetActive(true);
		scorePanel.SetActive(true);
		optionPanel.SetActive(true);

		if (!game)
			game = FindObjectOfType<Game>();
		if (game != null)
		{
			int totalNewChevrons = playerScore + playerLost;
			if ((bPlayerWin == false) && (playerLost != 0))
				totalNewChevrons /= playerLost;

			game.UpdateChevronAccount(totalNewChevrons);

			string conference = "+";
			if (totalNewChevrons < 0)
				conference = " ";
			totalText.text = conference + totalNewChevrons.ToString();
		}
		
		game.SaveGame();

		Time.timeScale = 0.1f;
	}

	public void Continue()
	{
		SceneManager.LoadScene("CampaignScene");
	}

	public void Return()
	{
		ObjectiveType[] objectives = FindObjectsOfType<ObjectiveType>();
		int numObjs = objectives.Length;
		for(int i = 0; i < numObjs; i++)
		{
			GameObject obj = objectives[i].gameObject;
			Destroy(obj);
		}

		Scanner[] scanners = FindObjectsOfType<Scanner>();
		int numScanners = scanners.Length;
		for(int i = 0; i < numScanners; i++)
		{
			GameObject scn = scanners[i].gameObject;
			Destroy(scn);
		}
		SceneManager.LoadScene("FleetScene");
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
