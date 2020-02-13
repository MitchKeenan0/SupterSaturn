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
	private IEnumerator disengageDelay;

	void Awake()
	{
		game = FindObjectOfType<Game>();
		tutorialPanel.SetActive(false);
		escapeMenuPanel.SetActive(false);
		disengageDelayPanel.SetActive(false);
		if (FindObjectOfType<BattleFleet>())
			disengageButton.interactable = true;
		else
			disengageButton.interactable = false;
	}

	public void SetTutorialActive(bool value)
	{
		tutorialPanel.SetActive(value);
		if (!game)
			game = FindObjectOfType<Game>();

		if ((game != null) && (value == false))
		{
			game.CloseTutorial();
		}
	}

	public void SetEscapeMenuActive(bool value)
	{
		escapeMenuPanel.SetActive(value);
	}

	public void CloseEscapeMenu()
	{
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
			Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, (time + 0.2f) * Time.deltaTime);
		}
		SceneManager.LoadScene("FleetScene");
	}

	public void ExitToDesktop()
	{
		game.SaveGame();
		Application.Quit();
	}
}
