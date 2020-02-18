using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHUD : MonoBehaviour
{
	public GameObject tutorialPanel;
	public GameObject escapeMenuPanel;
	public Button disengageButton;
	public GameObject disengageDelayPanel;
	public Text disengageDelayText;

	private Game game;
	private OptionsMenu options;
	private MainMenu menu;
	private TurnManager turnManager;
	private CampaignHud campaignHud;
	private TeamFleetHUD teamHud;
	private IEnumerator disengageDelay;

	void Awake()
	{
		game = FindObjectOfType<Game>();
		if (Resources.FindObjectsOfTypeAll<OptionsMenu>().Length > 0)
			options = Resources.FindObjectsOfTypeAll<OptionsMenu>()[0];
		tutorialPanel.SetActive(false);
		escapeMenuPanel.SetActive(false);
		disengageDelayPanel.SetActive(false);
		if (FindObjectOfType<BattleFleet>())
			disengageButton.interactable = true;
		else
			disengageButton.interactable = false;
		ClockObjects();
	}

	void ClockObjects()
	{
		menu = FindObjectOfType<MainMenu>();
		turnManager = FindObjectOfType<TurnManager>();
		campaignHud = FindObjectOfType<CampaignHud>();
		teamHud = FindObjectOfType<TeamFleetHUD>();
	}

	public void SetTutorialActive(bool value)
	{
		tutorialPanel.SetActive(value);

		if (!game)
			game = FindObjectOfType<Game>();

		if ((game != null) && (value == false))
			game.CloseTutorial();
	}

	public void SetEscapeMenuActive(bool value)
	{
		ClockObjects();
		if (menu != null)
			menu.SetMenuPanelVisible(!value);
		if (turnManager != null)
			turnManager.SetPanelActive(!value);
		if (campaignHud != null)
			campaignHud.SetPanelActive(!value);
		if (teamHud != null)
			teamHud.gameObject.SetActive(!value);

		escapeMenuPanel.SetActive(value);
	}

	public void CloseEscapeMenu()
	{
		ClockObjects();
		if (menu != null)
			menu.SetMenuPanelVisible(true);
		if (turnManager != null)
			turnManager.SetPanelActive(true);
		if (campaignHud != null)
			campaignHud.SetPanelActive(true);
		if (teamHud != null)
			teamHud.gameObject.SetActive(true);

		game.EscapeMenu();
	}

	public void Disengage()
	{
		disengageDelayPanel.SetActive(true);
		disengageDelayText.text = "1.00";
		game.SaveGame();
		disengageDelay = DisengageTimer(1f);
		StartCoroutine(disengageDelay);
	}

	private IEnumerator DisengageTimer(float waitTime)
	{
		float time = 0;
		while (time < waitTime)
		{
			float deltaT = Time.deltaTime;
			yield return new WaitForSeconds(deltaT);
			disengageDelayText.text = (1f - time).ToString("F2");
			time += deltaT;
			Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, (time + 0.618f) * Time.deltaTime);
		}
		SceneManager.LoadScene("FleetScene");
	}

	public void Options()
	{
		options.EnterOptions();
	}

	public void ReturnToMainMenu()
	{
		Time.timeScale = 1;
		game.SaveGame();
		SceneManager.LoadScene("BaseScene");
	}

	public void ExitToDesktop()
	{
		Time.timeScale = 1;
		game.SaveGame();
		Application.Quit();
	}
}
