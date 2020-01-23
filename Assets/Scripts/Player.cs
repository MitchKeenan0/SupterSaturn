using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public string playerName = "No Name";

	private Game game;
	private Player player;

	private void Awake()
	{
		if (player == null)
		{
			DontDestroyOnLoad(gameObject);
			player = this;
		}
		else if (game != this)
		{
			Destroy(gameObject);
		}

		game = FindObjectOfType<Game>();
	}

	public void SetName(string name)
	{
		playerName = name;
		game.SaveGame();
	}
}
