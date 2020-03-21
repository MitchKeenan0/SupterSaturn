using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
	private Game game;

	void Start()
	{
		game = FindObjectOfType<Game>();
		int numSpacecraft = transform.childCount;
		if (numSpacecraft == 0)
		{
			GameObject spPrefab = game.GetSelectedCard().cardObjectPrefab;
			Instantiate(spPrefab, transform);
		}
	}
}
