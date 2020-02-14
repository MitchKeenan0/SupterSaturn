using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject gameLoadingPanel;
	public GameObject fleetLoadingPanel;
	public GameObject menuPanel;

	private Game game;
	private OptionsMenu options;

    void Start()
    {
		gameLoadingPanel.SetActive(false);
		fleetLoadingPanel.SetActive(false);
		options = FindObjectOfType<OptionsMenu>();
		options.gameObject.SetActive(false);
		game = FindObjectOfType<Game>();
		game.LoadGame();
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
