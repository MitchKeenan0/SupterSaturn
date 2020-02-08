using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
	public GameObject tutorialPanel;

	private Game game;

	void Awake()
	{
		game = FindObjectOfType<Game>();
		tutorialPanel.SetActive(false);
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
}
