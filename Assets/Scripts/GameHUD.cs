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
	}

	public void SetTutorialActive(bool value)
	{
		tutorialPanel.SetActive(value);
		if (game == null)
			game = FindObjectOfType<Game>();

		if (game != null)
			game.CloseTutorial();
	}
}
