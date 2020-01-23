using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	public static Game game;

	public Card[] cardLibrary;
	public List<Card> initialSpacecraftCards;
	public int initialChevrons = 1000;

	public List<Card> GetCards() { return availableCardList; }
	public List<Card> GetSelectedCards() { return selectedCardList; }
	public List<Spacecraft> GetSpacecraftList() { return spacecraftList; }
	public void AddCard(Card value) { if (!availableCardList.Contains(value)) { availableCardList.Add(value); } }
	public int GetChevrons() { return chevrons; }
	public void UpdateChevronAccount(int value) { chevrons += value; }

	private Player player;
	private Fleet playerFleet;
	private Fleet enemyFleet;
	private List<Card> availableCardList;
	private List<Card> selectedCardList;
	private List<Spacecraft> spacecraftList;
	private int currentMission = 0;
	private int chevrons = 0;

	void Awake()
	{
		if (game == null)
		{
			DontDestroyOnLoad(gameObject);
			game = this;
		}
		else if (game != this)
		{
			Destroy(gameObject);
		}

		availableCardList = new List<Card>();
		selectedCardList = new List<Card>();
		spacecraftList = new List<Spacecraft>();

		player = FindObjectOfType<Player>();

		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	public void SetSelectedCard(int index, Card card)
	{
		if (index < selectedCardList.Count)
		{
			selectedCardList[index] = card;
			//Debug.Log("setting " + index + " with selected card " + card.cardName + " of " + selectedCardList.Count + " selected cards");
		}
		else
		{
			selectedCardList.Add(card);
			//Debug.Log("Adding new index " + index + " with selected card " + card.cardName + " of " + selectedCardList.Count + " selected cards");
		}
	}

	public void AddSpacecraft(Spacecraft sp)
	{
		spacecraftList.Add(sp);
	}

	public void LoadGame()
	{
		if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			// player name
			if (save.playerName != null)
			{
				player.SetName(save.playerName);
			}

			// player cards
			if (save.cardIDList != null)
			{
				int savedCardCount = save.cardIDList.Count;
				for (int i = 0; i < savedCardCount; i++)
				{
					SetSelectedCard(i, cardLibrary[save.cardIDList[i]]);
				}

				Debug.Log("Game Loaded");
			}
			else
			{
				Debug.Log("save cards uninitialized");
			}

			// health
			// ...

			// chevrons
			chevrons = save.chevrons;
			if (chevrons == 0)
				chevrons = initialChevrons;
		}
	}

	public void SaveGame()
	{
		Save save = CreateSaveGameObject();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");

		// player name
		save.playerName = player.playerName;

		// spacecraft cards
		if (GetSelectedCards().Count > 0)
		{
			int numCards = GetSelectedCards().Count;
			for (int i = 0; i < numCards; i++)
			{
				int cardIndex = GetSelectedCards()[i].numericID;
				if (i < save.cardIDList.Count)
					save.cardIDList[i] = cardLibrary[cardIndex].numericID;
				else
					save.cardIDList.Add(cardLibrary[cardIndex].numericID);
			}
		}
		else
		{
			int numCards = initialSpacecraftCards.Count;
			for (int i = 0; i < numCards; i++)
			{
				save.cardIDList.Add(cardLibrary[i].numericID);
			}
		}

		// spacecraft health
		int numSpacecraft = spacecraftList.Count;
		for (int i = 0; i < numSpacecraft; i++)
			save.spacecraftHealthList.Add(spacecraftList[i].GetHealth());

		// chevrons
		save.chevrons = game.GetChevrons();

		bf.Serialize(file, save);
		file.Close();

		Debug.Log("Game Saved");
	}

	private Save CreateSaveGameObject()
	{
		Save save = new Save();

		save.cardIDList = new List<int>();
		save.spacecraftHealthList = new List<int>();

		// player name
		save.playerName = player.playerName;

		// spacecraft cards
		if (GetSelectedCards().Count > 0)
		{
			int numCards = GetSelectedCards().Count;
			for (int i = 0; i < numCards; i++)
				save.cardIDList.Add(cardLibrary[GetSelectedCards()[i].numericID].numericID);
		}
		else
		{
			int numCards = initialSpacecraftCards.Count;
			for (int i = 0; i < numCards; i++)
			{
				int initialCardID = initialSpacecraftCards[i].numericID;
				save.cardIDList.Add(initialCardID);
			}
		}

		// spacecraft health
		int numSpacecraft = spacecraftList.Count;
		for(int i = 0; i < numSpacecraft; i++)
			save.spacecraftHealthList.Add(spacecraftList[i].GetHealth());

		// chevrons
		save.chevrons = game.GetChevrons();

		return save;
	}

	private string GetSaveFilePath
	{
		get { return Application.persistentDataPath + "/gamesave.save"; }
	}

	public void DeleteSave()
	{
		try
		{
			File.Delete(GetSaveFilePath);
			Debug.Log("save deleted");
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}
}
