using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject gameLoadingPanel;
	public GameObject menuPanel;

	private Game game;
	private OptionsMenu options;

    void Start()
    {
		gameLoadingPanel.SetActive(false);
		options = FindObjectOfType<OptionsMenu>();
		options.gameObject.SetActive(false);
		game = FindObjectOfType<Game>();
		game.LoadGame();
	}

	public void StartGame()
	{
		gameLoadingPanel.SetActive(true);
		game.SetGameMode(2);
		game.LoadScene("BayScene");
	}

	public void StartBattle()
	{
		gameLoadingPanel.SetActive(true);
		game.SetGameMode(1);
		game.LoadScene("BattleScene");
	}

	public void StartCampaign()
	{
		gameLoadingPanel.SetActive(true);
		game.SetGameMode(3);
		game.LoadScene("CampaignScene");
	}

	public void FleetCreator()
	{
		gameLoadingPanel.SetActive(true);
		game.SetGameMode(0);
		game.LoadScene("FleetScene");
	}

	public void Options()
	{
		options.EnterOptions();
	}

	public void ExitGame()
	{
		game.SaveGame();
		Application.Quit();
	}

	public void SetMenuPanelVisible(bool value)
	{
		menuPanel.SetActive(value);
		this.enabled = value;
	}
}
