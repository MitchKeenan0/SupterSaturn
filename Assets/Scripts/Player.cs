﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public string playerName = "";
	public float reputation = 1f;

	private Game game;

	public static Player player;
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
		//game.SaveGame();
	}
}
