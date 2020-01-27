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
	public int initialChevrons = 200;
	
	public List<Card> GetSelectedCards() { return selectedCardList; }
	public int GetSavedHealth(int index) { return (savedHealthList.Count > index) ? savedHealthList[index] : -1; }
	public List<Spacecraft> GetSpacecraftList() { return spacecraftList; }
	public int GetChevrons() { return chevrons; }
	public void UpdateChevronAccount(int value) { chevrons += value; }

	private Player player;
	private Fleet playerFleet;
	private Fleet enemyFleet;
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

	public void SetSavedHealth(int index, int value)
	{
		///Debug.Log("Setting saved health " + value);
		if (index < savedHealthList.Count)
			savedHealthList[index] = value;
		else
			savedHealthList.Add(value);
		Debug.Log("Setting saved health " + index + " of " + savedHealthList.Count + " to " + value);
	}

	public void RemoveSelectedCard(int index)
	{
		if ((index >= 0) && (index < selectedCardList.Count) && (selectedCardList.Count > 0))
			selectedCardList.RemoveAt(index);
		RemoveSpacecraft(index);
	}

	public void AddSpacecraft(Spacecraft sp, Card card)
	{
		Spacecraft spacecraft = sp;
		if (!spacecraft)
			spacecraft = CreateSpacecraft(card);
		if (spacecraft != null)
		{
			if (!spacecraftList.Contains(spacecraft))
			{
				spacecraftList.Add(spacecraft);
				Debug.Log("Game added " + spacecraft.spacecraftName + " to spacecraft list to make " + spacecraftList.Count);
			}
		}
		else
		{
			Debug.Log("Game AddSpacecraft was null");
		}
	}

	public void RemoveSpacecraft(int index)
	{
		if (index < spacecraftList.Count)
		{
			spacecraftList[index] = null;
			spacecraftList.RemoveAt(index);
			Debug.Log("Game removed spacecraft index " + index);
		}
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
			spacecraftList.Clear();

			// player name
			string playerName = save.playerName;
			if (playerName == "")
				playerName = "No Name";
			player.SetName(playerName);

			// player cards
			int savedCardCount = save.cardList.Count;
			if (savedCardCount > 0)
			{
				for (int i = 0; i < savedCardCount; i++)
				{
					int selectedCardID = save.cardList[i];
					Card selectedCard = cardLibrary[selectedCardID];
					SetSelectedCard(i, selectedCard);
					AddSpacecraft(null, selectedCard);
				}
			}

			// health
			int numHealth = save.healthList.Count;
			for (int i = 0; i < numHealth; i++)
			{
				if (i < selectedCardList.Count)
				{
					int savedHealth = save.healthList[i];
					Debug.Log("Load  saved health " + savedHealth);
					SetSavedHealth(i, savedHealth);
					if (i < spacecraftList.Count)
					{
						int maxHealth = spacecraftList[i].GetComponent<Health>().maxHealth;
						int spawnDamage = savedHealth - maxHealth;
						///Debug.Log("LoadGame spawn damage: " + spawnDamage);
						spacecraftList[i].GetComponent<Health>().ModifyHealth(spawnDamage, null);
					}
				}
			}

			// chevrons
			chevrons = save.chevrons;
			if (chevrons <= 0)
				chevrons = initialChevrons;

			///Debug.Log("Game Loaded");
		}
	}

	Spacecraft CreateSpacecraft(Card card)
	{
		Spacecraft sp = null;
		GameObject spacecraftObject = Instantiate(card.cardObjectPrefab, null);
		spacecraftObject.SetActive(false);
		sp = spacecraftObject.GetComponent<Spacecraft>();
		Debug.Log("Game createspacecraft adding spacecraft " + card.cardName);
		return sp;
	}

	public void SaveGame()
	{
		Save save = CreateSaveGameObject();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
		bf.Serialize(file, save);
		file.Close();
	}

	private Save CreateSaveGameObject()
	{
		Save save = new Save();

		// player name
		if (player != null)
		{
			string playerName = player.playerName;
			if (playerName == "")
				playerName = "No Name";
			save.playerName = player.playerName;
		}
		
		// spacecraft cards
		int numCards = selectedCardList.Count;
		for (int i = 0; i < numCards; i++)
		{
			int cardIndex = selectedCardList[i].numericID;
			save.cardList.Add(cardLibrary[cardIndex].numericID);
		}
		if (save.cardList.Count == 0)
		{
			save.cardList.Add(initialSpacecraftCards[0].numericID);
		}

		// spacecraft health
		int numSpacecraft = spacecraftList.Count;
		Debug.Log("Saving " + numSpacecraft + " spacecraft healths");
		if (numSpacecraft > 0)
		{
			for (int i = 0; i < numSpacecraft; i++)
			{
				if (spacecraftList[i] != null)
				{
					int spacecraftHealth = spacecraftList[i].GetHealth();
					save.healthList.Add(spacecraftHealth);
				}
			}
		}

		// chevrons
		if (chevrons <= 0)
			chevrons = initialChevrons;
		save.chevrons = chevrons;

		///Debug.Log("Game Saved");

		return save;
	}

	public void DeleteSave()
	{
		try
		{
			selectedCardList.Clear();
			savedHealthList.Clear();
			spacecraftList.Clear();

			File.Delete(GetSaveFilePath);

			SaveGame();

			///Debug.Log("save deleted");
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
