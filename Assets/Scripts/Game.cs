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
	public Card[] npcCardLibrary;
	public List<Card> initialSpacecraftCards;
	public int initialChevrons = 200;
	public Campaign ghostCampaignPrefab;

	private Player player;
	private Fleet playerFleet;
	private Fleet enemyFleet;
	private GameHUD gameHud;
	private Campaign campaign;
	private GameObject ghostCampaign;
	private List<Card> selectedCardList;
	private List<Card> enemyCardList;
	private List<Spacecraft> spacecraftList;
	private List<Spacecraft> enemySpacecraftList;
	private List<int> savedHealthList;
	private int sceneSetting = 0;
	private int chevrons = 0;
	private int gameMode = -1;
	private bool bEscapeMenu = false;
	private bool bFleetTutorialClosed = false;
	private bool bGameTutorialClosed = false;

	public List<Card> GetSelectedCards() { return selectedCardList; }
	public List<Card> GetEnemyCards() { return enemyCardList; }
	public List<Spacecraft> GetSpacecraftList() { return spacecraftList; }
	public List<Spacecraft> GetEnemySpacecraftList() { return enemySpacecraftList; }
	public int GetSavedHealth(int index) { return (savedHealthList.Count > index) ? savedHealthList[index] : -1; }
	public int GetChevrons() { return chevrons; }
	public int GetSceneSetting() { return sceneSetting; }
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
		selectedCardList = new List<Card>();
		enemyCardList = new List<Card>();
		spacecraftList = new List<Spacecraft>();
		enemySpacecraftList = new List<Spacecraft>();
		savedHealthList = new List<int>();

		player = FindObjectOfType<Player>();
		gameHud = FindObjectOfType<GameHUD>();
		campaign = FindObjectOfType<Campaign>();
	}

	private void OnApplicationPause(bool paused)
	{
		SaveGame();
	}

	void OnApplicationQuit()
	{
		SaveGame();
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
		gameHud = FindObjectOfType<GameHUD>();
		campaign = FindObjectOfType<Campaign>();
		Application.targetFrameRate = 30;
	}

	void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void Update()
	{
		if (Input.GetButtonDown("Cancel"))
			EscapeMenu();
	}

	public int GetGameMode() { return gameMode; }
	public void SetGameMode(int value)
	{
		gameMode = value;
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void LoadGameMode()
	{
		string sceneName = "";
		switch(gameMode)
		{
			case 1: sceneName = "BattleScene";
				break;
			case 2: sceneName = "BayScene";
				break;
			default:
				break;
		}
		if (sceneName != "")
			SceneManager.LoadScene(sceneName);
	}

	public void EscapeMenu()
	{
		bEscapeMenu = !bEscapeMenu;
		if (gameHud != null)
			gameHud.SetEscapeMenuActive(bEscapeMenu);
		else
			Debug.Log("no game menu");
		if (bEscapeMenu)
			Time.timeScale = Time.deltaTime * 5f;
		else
			Time.timeScale = 1f;
	}

	public void SetSelectedCard(int index, Card card)
	{
		if (selectedCardList == null)
			selectedCardList = new List<Card>();
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

		///Debug.Log("player has " + selectedCardList.Count + " selected cards");
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

		///Debug.Log("player has " + spacecraftList.Count + " spacecraft");
	}

	public void SetSavedHealth(int index, int value)
	{
		if (value != -1)
		{
			if (index < savedHealthList.Count)
				savedHealthList[index] = value;
			else
				savedHealthList.Add(value);
			///Debug.Log("Setting saved health " + index + " of " + savedHealthList.Count + " to " + value);
		}
		else if (index < savedHealthList.Count)
		{
			savedHealthList.Remove(index);
		}
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

	public void SetSceneSetting(int sceneTypeID)
	{
		sceneSetting = sceneTypeID;
	}

	public void LoadGame()
	{
		if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			if (selectedCardList != null)
			{
				selectedCardList.Clear();
				enemyCardList.Clear();
				spacecraftList.Clear();
				enemySpacecraftList.Clear();
				savedHealthList.Clear();
			}

			// player name
			if (player != null)
			{
				string playerName = save.playerName;
				if (playerName == "")
					playerName = "No Name";
				player.SetName(playerName);
			}

			// player cards
			int savedCardCount = save.cardList.Count;
			if (savedCardCount > 0)
			{
				for (int i = 0; i < savedCardCount; i++)
				{
					int selectedCardID = save.cardList[i];
					Card selectedCard = cardLibrary[selectedCardID];
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
				if (savedHealth == -1)
					savedHealth = spacecraftList[i].GetComponent<Health>().maxHealth;
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
					GameObject enemyObj = Instantiate(npcCardLibrary[cardIndex].cardObjectPrefab, transform);
					enemyObj.SetActive(false);
					Spacecraft sp = enemyObj.GetComponent<Spacecraft>();
					AddEnemy(sp);
				}
			}

			// chevrons
			chevrons = save.chevrons;
			if (chevrons <= initialChevrons)
				chevrons = initialChevrons;

			// tutorial
			bFleetTutorialClosed = save.fleetTutorialClosed;
			bGameTutorialClosed = save.gameTutorialClosed;
			gameHud = FindObjectOfType<GameHUD>();
			if (gameHud != null)
			{
				string sceneName = SceneManager.GetActiveScene().name;
				if (sceneName == "FleetScene")
				{
					if (bFleetTutorialClosed)
						gameHud.SetTutorialActive(false);
					//else
					//	gameHud.SetTutorialActive(true);
				}
				else if (sceneName == "GameScene")
				{
					if (bGameTutorialClosed)
						gameHud.SetTutorialActive(false);
					//else
					//	gameHud.SetTutorialActive(true);
				}
			}

			Debug.Log("Game Loaded");
		}
	}

	public void CloseTutorial()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		if (sceneName == "FleetScene")
			bFleetTutorialClosed = true;
		else if (sceneName == "GameScene")
			bGameTutorialClosed = true;
		SaveGame();
	}

	Spacecraft CreateSpacecraft(Card card)
	{
		Spacecraft sp = null;
		if (card != null)
		{
			GameObject spacecraftObject = Instantiate(card.cardObjectPrefab, null);
			spacecraftObject.SetActive(false);
			sp = spacecraftObject.GetComponent<Spacecraft>();
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
		Debug.Log("saving game...");
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

		int numEnemies = enemySpacecraftList.Count;
		if (numEnemies > 0)
		{
			for (int i = 0; i < numEnemies; i++)
			{
				if (i < enemyCardList.Count)
				{
					save.enemyCardList.Add(enemyCardList[i].numericID);
					///Debug.Log("Saved enemy card");
				}
			}
		}

		int numSpacecraft = selectedCardList.Count;
		///Debug.Log("Saving " + numSpacecraft + " spacecraft healths");
		if (numSpacecraft > 0)
		{
			for (int i = 0; i < numSpacecraft; i++)
			{
				FleetCreator fleetCreator = FindObjectOfType<FleetCreator>();
				// live spacecraft
				if (spacecraftList[i] != null)
				{
					if (spacecraftList[i].isActiveAndEnabled)
					{
						int spacecraftHealth = spacecraftList[i].GetComponent<Health>().GetHealth();
						if (spacecraftHealth == -1)
							spacecraftHealth = spacecraftList[i].GetComponent<Health>().maxHealth;
						save.healthList.Add(spacecraftHealth);
						///Debug.Log(spacecraftList[i].spacecraftName + " " + spacecraftHealth);
					}
					else if ((i < game.savedHealthList.Count)
						&& ((fleetCreator != null) || (campaign != null)))
					{
						int hp = 0;
						if (i < savedHealthList.Count)
							hp = game.savedHealthList[i];

						save.healthList.Add(hp);
						///Debug.Log(spacecraftList[i].spacecraftName + " " + hp);
					}
				}
				else
				{
					save.healthList.Add(0);
					///Debug.Log(spacecraftList[i].spacecraftName + " " + 0);
				}
			}
		}

		// chevrons
		if (chevrons <= 0)
			chevrons = initialChevrons;
		save.chevrons = chevrons;

		// tutorial
		save.fleetTutorialClosed = bFleetTutorialClosed;
		save.gameTutorialClosed = bGameTutorialClosed;

		Debug.Log("game saved");

		return save;
	}

	public void DeleteSave()
	{
		try
		{
			File.Delete(Application.persistentDataPath + "/gamesave.save");
			File.Delete(Application.persistentDataPath + "/campaignsave.save");

			Spacecraft[] allSpacecraft = FindObjectsOfType<Spacecraft>();
			int numSp = allSpacecraft.Length;
			for (int i = 0; i < numSp; i++)
				Destroy(allSpacecraft[i]);

			selectedCardList = new List<Card>();
			savedHealthList = new List<int>();
			spacecraftList = new List<Spacecraft>();
			enemyCardList = new List<Card>();
			enemySpacecraftList = new List<Spacecraft>();

			bFleetTutorialClosed = false;
			bGameTutorialClosed = false;

			SaveGame();

			///Debug.Log("save deleted");
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}
}
