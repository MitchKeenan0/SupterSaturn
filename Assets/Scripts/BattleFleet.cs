using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFleet : MonoBehaviour
{
	public int teamID = 0;
	public Vector3[] groupPositions;

	private Game game;
	private Player player;

	private IEnumerator loadWaitCoroutine;
    
	void Awake()
	{
		game = FindObjectOfType<Game>();
		player = FindObjectOfType<Player>();
	}

    void Start()
    {
		game.LoadGame();
		loadWaitCoroutine = LoadWait(0.2f);
		StartCoroutine(loadWaitCoroutine);
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (teamID == 0)
			InitSavedFleet();
	}

	void InitSavedFleet()
	{
		List<Card> cards = new List<Card>(game.GetSelectedCards());
		int numCards = cards.Count;
		for (int i = 0; i < numCards; i++)
		{
			if (i < 3)
			{
				GameObject cardObject = Instantiate(cards[i].cardObjectPrefab, null);
				cardObject.transform.position = transform.position + groupPositions[i];
				cardObject.transform.SetParent(transform);
				game.AddSpacecraft(cardObject.GetComponent<Spacecraft>());
			}
		}
	}
}
