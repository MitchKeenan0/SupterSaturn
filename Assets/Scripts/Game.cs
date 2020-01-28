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

	private Player player;
	private Fleet playerFleet;
	private Fleet enemyFleet;
	private List<Card> selectedCardList;
	private List<Card> enemyCardList;
	private List<Spacecraft> spacecraftList;
	private List<Spacecraft> enemySpacecraftList;
	private List<int> savedHealthList;
	private int currentMission = 0;
	private int chevrons = 0;

	public List<Card> GetSelectedCards() { return selectedCardList; }
	public List<Card> GetEnemyCards() { return enemyCardList; }
	public List<Spacecraft> GetSpacecraftList() { return spacecraftList; }
	public List<Spacecraft> GetEnemySpacecraftList() { return enemySpacecraftList; }
	public int GetSavedHealth(int index) { return (savedHealthList.Count > index) ? savedHealthList[index] : -1; }
	public int GetChevrons() { return chevrons; }
	public void UpdateChevronAccount(int value) { chevrons += value; }

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}
	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("scene loaded");
	}

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
		enemyCardList = new List<Card>();
		spacecraftList = new List<Spacecraft>();
		enemySpacecraftList = new List<Spacecraft>();
		savedHealthList = new List<int>();
		player = FindObjectOfType<Player>();
		Application.targetFrameRate = 70;
		Debug.Log("awake");
	}

	void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	public void SetSelectedCard(int index, Card card)
	{
		if (index < selectedCardList.Count)
		{
			if (card == null)
				selectedCardList.RemoveAt(index);
			else
				selectedCardList[index] = card;
		}
		else if (card != null)
		{
			selectedCardList.Add(card);
		}
	}

	public void SetSpacecraft(int index, Spacecraft sp)
	{
		if (index < spacecraftList.Count)
		{
			if (sp == null)
				spacecraftList.RemoveAt(index);
			else
				spacecraftList[index] = sp;
		}
		else if (sp != null)
		{
			spacecraftList.Add(sp);
		}
	}

	public void SetSavedHealth(int index, int value)
	{
		if (index < savedHealthList.Count)
			savedHealthList[index] = value;
		else if (value != -1)
			savedHealthList.Add(value);
		///Debug.Log("Setting saved health " + index + " of " + savedHealthList.Count + " to " + value);
	}

	public void SetEnemyCard(int index, Card card)
	{
		if (index < enemyCardList.Count)
			enemyCardList[index] = card;
		else if (card != null)
			enemyCardList.Add(card);
	}

	public void AddEnemy(Spacecraft sp)
	{
		if (!enemySpacecraftList.Contains(sp))
			enemySpacecraftList.Add(sp);
	}

	public void RemoveEnemy(Spacecraft sp)
	{
		if (enemySpacecraftList.Contains(sp))
			enemySpacecraftList.Remove(sp);
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
			enemyCardList.Clear();
			spacecraftList.Clear();
			enemySpacecraftList.Clear();
			savedHealthList.Clear();

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
					Debug.Log("loading card " + selectedCard.cardName);
					SetSelectedCard(i, selectedCard);
					Spacecraft sp = CreateSpacecraft(selectedCard);
					SetSpacecraft(i, sp);
				}
			}

			// health
			int savedHealthCount = save.healthList.Count;
			for (int i = 0; i < savedHealthCount; i++)
			{
				int savedHealth = save.healthList[i];
				Debug.Log("Loading health " + savedHealth);
				SetSavedHealth(i, savedHealth);

				if (i < spacecraftList.Count)
				{
					int maxHealth = spacecraftList[i].GetHealth();
					int spawnDamage = savedHealth - maxHealth;
					///Debug.Log("LoadGame spawn damage: " + spawnDamage);
					spacecraftList[i].GetComponent<Health>().ModifyHealth(spawnDamage, null);
				}
			}

			// enemy cards
			int savedEnemies = save.enemyCardList.Count;
			if (savedEnemies > 0)
			{
				for(int i = 0; i < savedEnemies; i++)
				{
					int cardIndex = save.enemyCardList[i];
					GameObject enemyObj = Instantiate(cardLibrary[cardIndex].cardObjectPrefab, transform);
					enemyObj.SetActive(false);
					Spacecraft sp = enemyObj.GetComponent<Spacecraft>();
					AddEnemy(sp);
				}
			}

			// chevrons
			chevrons = save.chevrons;
			if (chevrons <= 0)
				chevrons = initialChevrons;

			///Debug.Log("Game Loaded");
		}
	}

	void ClearAll()
	{
		//foreach(Spacecraft sp in spacecraftList)

	}

	Spacecraft CreateSpacecraft(Card card)
	{
		Spacecraft sp = null;
		if (card != null)
		{
			GameObject spacecraftObject = Instantiate(card.cardObjectPrefab, null);
			spacecraftObject.SetActive(false);
			sp = spacecraftObject.GetComponent<Spacecraft>();
			Debug.Log("Game CreateSpacecraft " + card.cardName);
		}
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
		
		// player cards
		int numCards = selectedCardList.Count;
		for (int i = 0; i < numCards; i++)
		{
			if (selectedCardList[i] != null)
			{
				int cardIndex = selectedCardList[i].numericID;
				save.cardList.Add(cardLibrary[cardIndex].numericID);
			}
		}
		if (save.cardList.Count == 0)
		{
			save.cardList.Add(initialSpacecraftCards[0].numericID);
		}

		// enemy cards
		int numEnemies = enemySpacecraftList.Count;
		if (numEnemies > 0)
		{
			for (int i = 0; i < numEnemies; i++)
			{
				if (i < enemyCardList.Count)
				{
					save.enemyCardList.Add(enemyCardList[i].numericID);
					Debug.Log("Saved enemy");
				}
			}
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
					int spacecraftHealth = spacecraftList[i].GetComponent<Health>().GetHealth();
					if (spacecraftHealth == -1)
						spacecraftHealth = spacecraftList[i].GetComponent<Health>().maxHealth;
					save.healthList.Add(spacecraftHealth);
					Debug.Log(spacecraftList[i].spacecraftName + " " + spacecraftHealth);
				}
			}
		}
		if (save.healthList.Count == 0)
		{
			int numSelected = selectedCardList.Count;
			Debug.Log("Saving " + numSelected + " spacecraft card healths");
			for (int i = 0; i < numSelected; i++)
			{
				int spacecraftHealth = selectedCardList[i].health;
				save.healthList.Add(spacecraftHealth);
				Debug.Log(spacecraftList[i].spacecraftName + " " + spacecraftHealth);
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
			File.Delete(GetSaveFilePath);

			selectedCardList.Clear();
			savedHealthList.Clear();
			spacecraftList.Clear();

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
