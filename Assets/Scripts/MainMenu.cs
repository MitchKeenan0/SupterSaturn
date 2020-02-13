using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject gameLoadingPanel;
	public GameObject fleetLoadingPanel;

	private Game game;

    void Start()
    {
		gameLoadingPanel.SetActive(false);
		fleetLoadingPanel.SetActive(false);
		game = FindObjectOfType<Game>();
		game.LoadGame();
	}

	public void GameLoading()
	{
		gameLoadingPanel.SetActive(true);
	}

	public void FleetLoading()
	{
		fleetLoadingPanel.SetActive(true);
	}

	public void StartGame()
	{
		gameLoadingPanel.SetActive(true);
		SceneManager.LoadScene("CampaignScene");
	}

	public void FleetCreator()
	{
		fleetLoadingPanel.SetActive(true);
		SceneManager.LoadScene("FleetScene");
	}
}
