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
	public int GetSavedHealth(int index) { return (savedHealthList.Count > index) ? savedHealthList[index] : -1; }
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
	private List<int> savedHealthList;
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
		savedHealthList = new List<int>();

		player = FindObjectOfType<Player>();

		DontDestroyOnLoad(gameObject);
		Application.targetFrameRate = 70;
	}

	void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	public void SetSelectedCard(int index, Card card)
	{
		if (index < selectedCardList.Count)
			selectedCardList[index] = card;
		else
			selectedCardList.Add(card);
	}

	public void RemoveSelectedCard(int index)
	{
		if (selectedCardList.Count > index)
			selectedCardList.RemoveAt(index);
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

			selectedCardList.Clear();
			savedHealthList.Clear();

			// player name
			string playerName = save.playerName;
			if (playerName == "")
				playerName = "No Name";
			player.SetName(playerName);

			// player cards
			int savedCardCount = save.cardIDList.Count;
			if (savedCardCount > 0)
			{
				for (int i = 0; i < savedCardCount; i++)
				{
					int selectedCardID = save.cardIDList[i];
					SetSelectedCard(i, cardLibrary[selectedCardID]);
				}
			}

			// health
			int numHealth = save.spacecraftHealthList.Count;
			for (int i = 0; i < numHealth; i++)
			{
				if (i < selectedCardList.Count)
				{
					Card spacecraftCard = selectedCardList[i];
					int savedHealth = save.spacecraftHealthList[i];
					spacecraftCard.SetHealth(savedHealth);
					savedHealthList.Add(savedHealth);
					Debug.Log("Loading health " + savedHealth);
				}
			}

			// chevrons
			chevrons = save.chevrons;
			if (chevrons == 0)
				chevrons = initialChevrons;

			Debug.Log("Game Loaded");
		}
	}

	public void SaveGame()
	{
		Save save = CreateSaveGameObject();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");

		Debug.Log("Game Saved");

		bf.Serialize(file, save);
		file.Close();
	}

	private Save CreateSaveGameObject()
	{
		Save save = new Save();

		// player name
		string playerName = player.playerName;
		if (playerName == "")
			playerName = "No Name";
		save.playerName = player.playerName;
		
		// spacecraft cards
		int numCards = selectedCardList.Count;
		for (int i = 0; i < numCards; i++)
		{
			int cardIndex = selectedCardList[i].numericID;
			save.cardIDList.Add(cardLibrary[cardIndex].numericID);
		}

		if (save.cardIDList.Count == 0)
		{
			numCards = initialSpacecraftCards.Count;
			for (int i = 0; i < numCards; i++)
			{
				save.cardIDList.Add(initialSpacecraftCards[i].numericID);
			}
		}

		// spacecraft health
		int numSpacecraft = spacecraftList.Count;
		for (int i = 0; i < numSpacecraft; i++)
		{
			if (spacecraftList[i] != null)
			{
				int spacecraftHealth = spacecraftList[i].GetHealth();
				//if (spacecraftHealth < 0)
				//	spacecraftHealth = spacecraftList[i].GetComponent<Health>().maxHealth;
				save.spacecraftHealthList.Add(spacecraftHealth);

				Debug.Log("Saved health " + spacecraftHealth);
			}
		}

		// chevrons
		save.chevrons = chevrons;

		return save;
	}

	public void DeleteSave()
	{
		try
		{
			File.Delete(GetSaveFilePath);
			Debug.Log("save deleted");
			game.SaveGame();
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private string GetSaveFilePath
	{
		get { return Application.persistentDataPath + "/gamesave.save"; }
	}
}
