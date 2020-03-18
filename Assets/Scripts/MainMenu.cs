using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject gameLoadingPanel;
	public GameObject menuPanel;

	private Game game;
	private ObjectiveBoard objBoard;
	private OptionsMenu options;
	private SoundManager soundManager;

    void Start()
    {
		gameLoadingPanel.SetActive(false);
		options = FindObjectOfType<OptionsMenu>();
		objBoard = FindObjectOfType<ObjectiveBoard>();
		options.gameObject.SetActive(false);
		game = FindObjectOfType<Game>();
		game.LoadGame();
		soundManager = FindObjectOfType<SoundManager>();
	}

	public void ObjectiveBoard(bool value)
	{
		objBoard.SetActive(value);
		menuPanel.SetActive(!value);
	}

	public void StartGame()
	{
		soundManager.AffirmativeButton();
		gameLoadingPanel.SetActive(true);
		game.SetGameMode(1);
		game.LoadScene("GameScene");
	}

	public void StartCampaign()
	{
		soundManager.AffirmativeButton();
		gameLoadingPanel.SetActive(true);
		game.SetGameMode(2);
		game.LoadScene("CampaignScene");
	}

	public void FleetCreator()
	{
		soundManager.AffirmativeButton();
		gameLoadingPanel.SetActive(true);
		game.SetGameMode(0);
		game.LoadScene("FleetScene");
	}

	public void Options()
	{
		soundManager.AffirmativeButton();
		options.EnterOptions();
	}

	public void ExitGame()
	{
		soundManager.AffirmativeButton();
		game.SaveGame();
		Application.Quit();
	}

	public void SetMenuPanelVisible(bool value)
	{
		menuPanel.SetActive(value);
		this.enabled = value;
	}
}
